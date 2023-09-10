
using System.Runtime.CompilerServices;
using Manila.Scripting.Exceptions;
using Microsoft.ClearScript;

namespace Manila.Scripting.API {
	/// <summary>
	/// A Task that can be executed
	/// </summary>
	public class Task {
		/// <summary>
		/// The name of the task
		/// </summary>
		public string name { get; private set; }
		private List<string> dependencies;
		private Func<bool>? func;

		/// <summary>
		/// Creates a new task
		/// </summary>
		/// <param name="name">the name of the task</param>
		public Task(string name) {
			this.name = name;
			dependencies = new List<string>();
		}

		/// <summary>
		/// Sets the function that gets executed
		/// </summary>
		/// <param name="func">the function</param>
		/// <returns>true: task suceeded, false: task failed</returns>
		public void onExecute(dynamic func) {
			this.func = () => {
				var res = func();
				// Return true if nothing or undefined gets returned
				if (res.GetType() != typeof(bool)) return true;
				return res;
			};
			ScriptEngine.getInstance().registerTask(this);
		}
		/// <summary>
		/// Adds a dependecy task to this task
		/// </summary>
		/// <param name="task">the dependency</param>
		public Task dependsOn(string task) {
			dependencies.Add(task);
			return this;
		}

		public bool execute() {
			var se = ScriptEngine.getInstance();

			foreach (string s in dependencies) {
				if (!se.hasTask(s)) throw new TaskNotFoundException(s, this);
				se.getTask(s).execute();
			}

			// Function cannot be null, as task only gets registered when the functon has been set
			return this.func.Invoke();
		}
	}
}
