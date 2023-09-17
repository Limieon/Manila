
using System.Text.RegularExpressions;
using Manila.Core.Exceptions;
using Manila.Data;
using Manila.Scripting;
using Manila.Scripting.API;
using Manila.Scripting.Exceptions;
using Manila.Utils;

namespace Manila.Core;

public static class ScriptManager {
	public enum State {
		NONE,
		INITIALIZING,
		RUNNING,
		NO_SCRIPTS
	}
	public enum Scope {
		NONE,
		WORKSPACE,
		PROJECT
	}

	internal static List<Scripting.API.Task> tasks = new List<Scripting.API.Task>();
	internal static Core.Workspace? workspace;
	internal static BuildConfig? buildConfig = null;

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
	/// <summary>
	/// Runs a task by it's name
	/// </summary>
	/// <param name="task">the name of the task</param>
	/// <returns>true: task succeded, false: task failed</returns>
	public static bool runTask(string task) {
		return true;
	}

	public static void addObject(string name, object obj) {
		engine.engine.AddHostObject(name, obj);
	}
	public static void addType(Type type) {
		engine.engine.AddHostType(type);
	}
	public static void addType(string name, Type type) {
		engine.engine.AddHostType(type);
	}
}
