
using Manila.Core;
using Manila.Scripting.Exceptions;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;

namespace Manila.Scripting.API;

/// <summary>
/// A Task that can be executed
/// </summary>
public class Task {
	/// <summary>
	/// The name of the task
	/// </summary>
	public string name { get; private set; }
	private List<string> dependencies;
	private ScriptObject func;

	internal readonly Project? project;

	/// <summary>
	/// Creates a new task
	/// </summary>
	/// <param name="name">the name of the task</param>
	public Task(string name) {
		if (ScriptManager.scope == ScriptManager.Scope.PROJECT) project = (Project) ScriptManager.currentScriptInstance;

		this.name = ":" + name;
		dependencies = new List<string>();
	}

	/// <summary>
	/// Sets the function that gets executed
	/// </summary>
	/// <param name="func">the function</param>
	/// <returns>true: task suceeded, false: task failed</returns>
	public void onExecute(dynamic func) {
		this.func = func; /*() => {
			var res = func();
			// Return true if nothing or undefined gets returned
			if (res.GetType() != typeof(bool)) return true;
			return res;
		};*/
		ScriptManager.registerTask(this);
	}
	/// <summary>
	/// Adds a dependecy task to this task
	/// </summary>
	/// <param name="task">the dependency</param>
	public Task dependsOn(string task) {
		dependencies.Add(task);
		return this;
	}

	/// <summary>
	/// Executes this task
	/// </summary>
	/// <returns>false: task returned false, true: task did not return false</returns>
	/// <exception cref="TaskNotFoundException"></exception>
	public async Task<bool> execute() {
		// Function cannot be null, as task only gets registered when the functon has been set
		var res = await func.InvokeAsFunction().ToTask();

		Console.WriteLine("Task Done!");
		return res.GetType() != typeof(bool) ? true : (bool) res;
	}

	/// <summary>
	/// Returns a list of directly dependant tasks
	/// </summary>
	/// <exception cref="TaskNotFoundException"></exception>
	public List<Task> getDependencies() {
		List<Task> res = new List<Task>();
		foreach (var d in dependencies) {
			if (!ScriptManager.hasTask(d)) throw new TaskNotFoundException(d, this);
			res.Add(ScriptManager.getTask(d));
		}
		return res;
	}
}
