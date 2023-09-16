
using Manila.Scripting;
using Manila.Scripting.API;
using Manila.Scripting.Exceptions;
using Manila.Utils;

namespace Manila.Core;

public static class ScriptManager {
	public enum State {
		NONE,
		INITIALIZING,
		RUNNING
	}
	public enum Scope {
		NONE,
		WORKSPACE,
		PROJECT
	}

	internal static List<Scripting.API.Task> tasks = new List<Scripting.API.Task>();
	internal static Workspace? workspace;

	internal static ScriptInstance? currentScriptInstance { get; set; }

	internal static readonly ScriptEngine engine = new ScriptEngine();

	internal static State state { get; private set; }
	internal static Scope scope { get; private set; }

	internal static void init() {
		state = State.INITIALIZING;
		scope = Scope.NONE;
	}

	internal static void runWorkspaceFile() {
		Logger.debug("Running workspace file...");

		workspace = new Workspace(new ManilaDirectory("."));
		currentScriptInstance = workspace;
		scope = Scope.WORKSPACE;
		engine.run("Manila.js");
		workspace.name = (string) currentScriptInstance.getProperty("name");

		Logger.debug("Workspace Name:", workspace.name);

		scope = Scope.PROJECT;
		runProjectFiles();
		scope = Scope.NONE;
	}
	private static void runProjectFiles() {
		Logger.debug("Running project files...");

		var files = workspace.location.files(true, (f) => {
			return f.EndsWith("Manila.js") && (new ManilaFile(f).getPathRelative(workspace.location.getPath()) != "Manila.js");
		});

		foreach (var f in files) {
			runProjectFile(f);
		}

		state = State.RUNNING;
	}

	private static void runProjectFile(ManilaFile file) {
		Logger.debug("Running project file", file.getPathRelative(workspace.location.getPath()));
		Project p = new Project(":" + file.getFileDirHandle().getPathRelative(workspace.location.getPath()).Replace(Path.PathSeparator, ':'), file.getFileDirHandle(), workspace);
		currentScriptInstance = p;

		Logger.debug($"ID: '{p.id}'");

		workspace.runFilters(p);

		engine.run(file.getPath());
		p.name = currentScriptInstance.getProperty("name");
		p.version = currentScriptInstance.getProperty("version");

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
		foreach (var t in ScriptManager.tasks) {
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
