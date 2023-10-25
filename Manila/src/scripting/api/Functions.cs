
using System.Text.RegularExpressions;
using Manila.Core;
using Manila.Utils;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Spectre.Console;

namespace Manila.Scripting.API;

/// <summary>
/// Default functions provided by the api
/// </summary>
public static class Functions {
	private static bool lastPrintLine = true;

	internal static void addToEngine(V8ScriptEngine e) {
		e.AddHostObject("task", task);
		e.AddHostObject("print", print);
		e.AddHostObject("markup", markup);
		e.AddHostObject("println", println);
		e.AddHostObject("markupln", markupln);
		e.AddHostObject("regex", regex);
		e.AddHostObject("properties", properties);
		e.AddHostObject("apply", apply);

		Action<ScriptObject, int> setTimeout = (func, delay) => {
			var timer = new Timer(_ => func.Invoke(false));
			timer.Change(delay, Timeout.Infinite);
		};
		e.Script._setTimeout = setTimeout;

		e.Execute(@"
			function setTimeout(func, delay) {
				let args = Array.prototype.slice.call(arguments, 2);
				_setTimeout(func.bind(undefined, ...args), delay || 0);
			}"
		);
		e.Execute(@"
			async function sleep(amount) { return new Promise((res, rej) => { setTimeout(res, amount) })}"
		);
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
		if (lastPrintLine && Scripting.API.Task.inTask) AnsiConsole.Write("  ");
		lastPrintLine = false;
		AnsiConsole.Write(string.Join(" ", text));
	}
	/// <summary>
	/// Prints text to stdout
	/// </summary>
	/// <param name="text">the text to print</param>
	public static void println(params dynamic[] text) {
		if (lastPrintLine && Scripting.API.Task.inTask) AnsiConsole.Write("  ");
		lastPrintLine = true;
		AnsiConsole.WriteLine(string.Join(" ", text));
	}
	/// <summary>
	/// Prints text to stdout (with markup suppor (visit: https://spectreconsole.net/markup))
	/// </summary>
	/// <param name="text">the text to print</param>
	public static void markup(params dynamic[] text) {
		if (lastPrintLine && Scripting.API.Task.inTask) AnsiConsole.Write("  ");
		lastPrintLine = false;
		AnsiConsole.Markup(string.Join(" ", text));
	}
	/// <summary>
	/// Prints text to stdout (with markup suppor (visit: https://spectreconsole.net/markup))
	/// </summary>
	/// <param name="text">the text to print</param>
	public static void markupln(params dynamic[] text) {
		if (lastPrintLine && Scripting.API.Task.inTask) AnsiConsole.Write("  ");
		lastPrintLine = true;
		AnsiConsole.MarkupLine(string.Join(" ", text));
	}

	/// <summary>
	/// Returns a new regex from a string
	/// </summary>
	/// <param name="exp">The regex expression</param>
	/// <returns></returns>
	public static Regex regex(string exp) {
		return new Regex(exp);
	}

	/// <summary>
	/// Sets properties to the current ScriptInstance
	/// </summary>
	/// <param name="obj">The properties to set</param>
	/// <exception cref="Exception">Gets thrown when no instance is currently set</exception>
	public static void properties(ScriptObject obj) {
		if (ScriptManager.currentScriptInstance == null) throw new Exception("Cannot set properties in the current context!");
		ScriptManager.currentScriptInstance.addProperties(ScriptUtils.toMap<dynamic>(obj));
	}

	public static void apply(string scriptTemplate) {
		ScriptManager.applyTemplate(scriptTemplate);
	}
}
