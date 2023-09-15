
using System.Text.RegularExpressions;
using Microsoft.ClearScript.V8;
using Spectre.Console;

namespace Manila.Scripting.API {
	/// <summary>
	/// Default functions provided by the api
	/// </summary>
	public static class Functions {
		internal static void addToEngine(V8ScriptEngine e) {
			e.AddHostObject("task", task);
			e.AddHostObject("print", print);
			e.AddHostObject("markup", markup);
			e.AddHostObject("regex", regex);
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
		/// Prints text to stdout
		/// </summary>
		/// <param name="text">the text to print</param>
		public static void print(params dynamic[] text) {
			AnsiConsole.WriteLine(string.Join(" ", text));
		}
		/// <summary>
		/// Prints text to stdout (with markup suppor (visit: https://spectreconsole.net/markup))
		/// </summary>
		/// <param name="text">the text to print</param>
		public static void markup(params dynamic[] text) {
			AnsiConsole.MarkupLine(string.Join(" ", text));
		}

		public static Regex regex(string exp) {
			return new Regex(exp);
		}
	}
}
