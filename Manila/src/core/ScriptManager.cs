
using System.Text.RegularExpressions;
using Manila.Core.Exceptions;
using Manila.Data;
using Manila.Scripting;
using Manila.Scripting.API;
using Manila.Scripting.Exceptions;
using Manila.Utils;

namespace Manila.Core;

/// <summary>
/// Provides functionality for managing and running build scripts
/// </summary>
public static class ScriptManager {
	/// <summary>
	/// Represents the possible states of the script manager
	/// </summary>
	public enum State {
		/// <summary>
		/// No state
		/// </summary>
		NONE,
		/// <summary>
		/// Initialization state
		/// </summary>
		INITIALIZING,
		/// <summary>
		/// Running state
		/// </summary>
		RUNNING,
		/// <summary>
		/// Not in a script env state
		/// </summary>
		NO_SCRIPTS
	}

	/// <summary>
	/// Represents the scope of script execution
	/// </summary>
	public enum Scope {
		/// <summary>
		/// No scope
		/// </summary>
		NONE,
		/// <summary>
		/// Workspace scope
		/// </summary>
		WORKSPACE,
		/// <summary>
		/// Project scope
		/// </summary>
		PROJECT
	}

	internal static List<Scripting.API.Task> tasks = new List<Scripting.API.Task>();
	internal static Core.Workspace? workspace;
	internal static BuildConfig? buildConfig = null;

	public static long timeBuildStarted { get; private set; }

	/// <summary>
	/// The data stored in the workspace.manila file
	/// </summary>
	public static Data.Workspace? workspaceData = null;

	internal static ScriptInstance? currentScriptInstance { get; set; }

	internal static readonly ScriptEngine engine = new ScriptEngine();

	internal static State state { get; private set; }
	internal static Scope scope { get; private set; }

	internal static void init(Data.Workspace workspace) {
		workspaceData = workspace;

		state = State.INITIALIZING;
		scope = Scope.NONE;
	}

	internal static void runWorkspaceFile() {
		Logger.debug("Running workspace file...");

		workspace = new Workspace(new ManilaDirectory("."));
		currentScriptInstance = workspace;
		scope = Scope.WORKSPACE;

		if (!new ManilaFile("Manila.js").exists()) {
			throw new Exception("No script environment!");
		}

		engine.run("Manila.js");
		workspace.name = workspaceData.data.name;

		Logger.debug("Workspace Name:", workspace.name);

		scope = Scope.PROJECT;
		runProjectFiles();
		scope = Scope.NONE;
	}
	private static void runProjectFiles() {
		Logger.debug("Running project files...");

		var files = new List<ManilaFile>();
		foreach (var p in workspaceData.data.projects) {
			var f = new ManilaFile(p, "Manila.js");
			if (!f.exists()) throw new FileNotFoundException("Project located in [blue]" + f.getFileDir() + "[/] does not contain a [yellow]Manila.js buildfile[/]!");
			files.Add(f);
		}

		foreach (var f in files) {
			runProjectFile(f);
		}

		state = State.RUNNING;
	}

	private static void runProjectFile(ManilaFile file) {
		Logger.debug("Running project file", file.getPathRelative(workspace.location.getPath()));
		var projectID = file.getFileDirHandle().getPathRelative(workspace.location.getPath());
		while (projectID.Contains('/')) {
			projectID = projectID.Replace('/', ':');
		}
		while (projectID.Contains('\\')) {
			projectID = projectID.Replace('\\', ':');
		}

		Project p = new Project(":" + projectID, file.getFileDirHandle(), workspace);
		currentScriptInstance = p;

		Logger.debug($"ID: '{p.id}'");

		workspace.runFilters(p);

		engine.run(file.getPath());

		try {
			p.name = currentScriptInstance.getProperty("name");
			p.version = currentScriptInstance.getProperty("version");
		} catch (KeyNotFoundException e) {
			throw new PropertyNotFoundException(p, new Regex("'(.*?)'").Match(e.Message).Captures[0].Value);
		}

		Logger.debug("Name:", p.name);
		Logger.debug("Version:", p.version);

		workspace.addProject(p);
	}

	internal static void shutdown() {
		engine.shutdown();
	}

	/// <summary>
	/// Registers a new task
	/// </summary>
	/// <param name="t">the task</param>
	public static void registerTask(Scripting.API.Task t) {
		tasks.Add(t);
	}
	/// <summary>
	/// Gets a task by it's name
	/// </summary>
	/// <param name="task">The name</param>
	/// <returns>the Task</returns>
	public static Scripting.API.Task getTask(string task) {
		List<string> tasks = new List<string>();
		foreach (var t in ScriptManager.tasks) {
			if (ScriptUtils.getTaskName(t) == task) return t;
			if (t.name == task || ScriptUtils.getTaskName(t) == task) tasks.Add(ScriptUtils.getTaskName(t));
		}

		if (tasks.Count == 0) throw new TaskNotFoundException(task);
		if (tasks.Count > 1) throw new Exception("Task '" + task + "' is not unique!");

		foreach (Scripting.API.Task t in ScriptManager.tasks) { if (ScriptUtils.getTaskName(t) == tasks[0]) return t; }

		throw new TaskNotFoundException(task);
	}
	/// <summary>
	/// Checks if a tasks existgs
	/// </summary>
	/// <param name="task">the task name</param>
	/// <returns>true: exists</returns>
	public static bool hasTask(string task) {
		foreach (var t in tasks) {
			if (t.name == task || ScriptUtils.getTaskName(t) == task) return true;
		}
		return false;
	}

	private static List<Scripting.API.Task> getTasksRec(Scripting.API.Task task, List<Scripting.API.Task> tasks = null) {
		if (tasks == null) tasks = new List<Scripting.API.Task>();

		foreach (var d in task.getDependencies())
			if (!tasks.Contains(d)) tasks = getTasksRec(d, tasks);

		if (!tasks.Contains(task)) tasks.Add(task);

		return tasks;
	}

	/// <summary>
	/// Executes a task by it's name (also runs dependant tasks)
	/// </summary>
	/// <param name="task">the task to execute</param>
	/// <returns>true: task succeded, false: task failed</returns>
	public static async Task<bool> executeTask(Scripting.API.Task task) {
		timeBuildStarted = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		var order = getTasksRec(task);

		var taskNum = 1;
		foreach (var t in order) {
			if (taskNum == order.Count) {
				Logger.infoMarkup(
					$"[green]{taskNum++}[/][gray]/[/][cyan]{order.Count}[/] [gray]>[/] [blue]{ScriptUtils.getTaskName(t)}[/]"
				);
			} else {
				Logger.infoMarkup(
					$"[yellow]{taskNum++}[/][gray]/[/][cyan]{order.Count}[/] [gray]>[/] [blue]{ScriptUtils.getTaskName(t)}[/]"
				);
			}

			await t.execute();
		}
		return true;
	}

	/// <summary>
	/// Adds an object to the script engine's host objects.
	/// </summary>
	/// <param name="name">The name of the object.</param>
	/// <param name="obj">The object to add.</param>
	public static void addObject(string name, object obj) {
		engine.engine.AddHostObject(name, obj);
	}
	/// <summary>
	/// Adds a type to the script engine's host types.
	/// </summary>
	/// <param name="type">The type to add.</param>
	public static void addType(Type type) {
		engine.engine.AddHostType(type);
	}
	/// <summary>
	/// Adds a type with a specified name to the script engine's host types.
	/// </summary>
	/// <param name="name">The name of the type.</param>
	/// <param name="type">The type to add.</param>
	public static void addType(string name, Type type) {
		engine.engine.AddHostType(type);
	}
}
