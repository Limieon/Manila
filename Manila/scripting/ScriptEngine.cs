
using Manila.Scripting.Exceptions;
using Microsoft.ClearScript.V8;

namespace Manila.Scripting {
	/// <summary>
	/// The main class that connects JS to C#
	/// </summary>
	public class ScriptEngine {
		private V8ScriptEngine engine;
		private List<API.Task> tasks;

		private static readonly ScriptEngine instance = new();
		private ScriptEngine() {
			engine = new V8ScriptEngine();
			engine.AddHostType("Console", typeof(Console));
			engine.AddHostType("Manila", typeof(API.Manila));

			tasks = new List<API.Task>();
		}

		/// <summary>
		/// Runs the main scripts
		/// </summary>
		/// <returns>ScriptEngine instance</returns>
		public ScriptEngine run() {
			engine.Execute(File.ReadAllText("./Manila.js"));
			return this;
		}

		/// <summary>
		/// Returns the instance to this class
		/// </summary>
		/// <returns>ScriptEngine</returns>
		public static ScriptEngine getInstance() { return instance; }

		/// <summary>
		/// Registers a new task
		/// </summary>
		/// <param name="t">the task</param>
		public void registerTask(API.Task t) {
			tasks.Add(t);
		}
		/// <summary>
		/// Gets a task by it's name
		/// </summary>
		/// <param name="task">The name</param>
		/// <returns>the Task</returns>
		public API.Task getTask(string task) {
			foreach (API.Task t in tasks) { if (t.name == task) return t; }
			throw new TaskNotFoundException(task);
		}
		/// <summary>
		/// Checks if a tasks existgs
		/// </summary>
		/// <param name="task">the task name</param>
		/// <returns>true: exists</returns>
		public bool hasTask(string task) {
			bool found = false;
			foreach (API.Task t in tasks) {
				if (t.name == task) {
					found = true;
					break;
				}
			}
			return found;
		}

		/// <summary>
		/// Runs a task by it's name
		/// </summary>
		/// <param name="task">the name of the task</param>
		/// <returns>true: task succeded, false: task failed</returns>
		public bool runTask(string task) {
			return true;
		}
	}
}
