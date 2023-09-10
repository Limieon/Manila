
using Microsoft.ClearScript.V8;
using Spectre.Console;

namespace Manila.Scripting.API {
	/// <summary>
	/// Default functions provided by the api
	/// </summary>
	public static class Functions {
		public static void addToEngine(V8ScriptEngine e) {
			e.AddHostObject("task", task);
			e.AddHostObject("print", print);
			e.AddHostObject("markup", markup);
		}

		/// <summary>
		/// Creates a new task
		/// </summary>
		/// <param name="name">the name of the task</param>
		/// <returns>the task object</returns>
		public static Task task(string name) {
			return new Task(name);
		}
		/// <summary>
		/// Prints text to console
		/// </summary>
		/// <param name="msg">the text to print</param>
		public static void print(params string[] msg) {
			AnsiConsole.WriteLine(string.Join(" ", msg));
		}
		/// <summary>
		/// Prints markup formatted text to console
		/// </summary>
		/// <param name="msg">the text to print</param>
		public static void markup(params string[] msg) {
			AnsiConsole.MarkupLine(string.Join(" ", msg));
		}
	}
}
