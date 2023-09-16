using Spectre.Console;

namespace Manila.Utils;

/// <summary>
/// A simple logger to hide verbose / debug messages
/// </summary>
public static class Logger {
	private static bool verbose = false;

	/// <summary>
	/// Initilizes the logger
	/// </summary>
	/// <param name="verbose">enables verbose logging</param>
	public static void init(bool verbose) {
		Logger.verbose = verbose;
	}

	/// <summary>
	/// Prints only if verbose has been enabled
	/// </summary>
	/// <param name="data">the data to print</param>
	public static void debug(params dynamic[] data) {
		if (!verbose) return;
		AnsiConsole.Markup("[cyan][[DEBUG]][/]: ");
		AnsiConsole.WriteLine(string.Join(" ", data));
	}
	/// <summary>
	/// Prints always
	/// </summary>
	/// <param name="data">the data to print</param>
	public static void info(params dynamic[] data) {
		Console.WriteLine(string.Join(" ", data));
	}

	/// <summary>
	/// Same as debug() but support for markup
	/// </summary>
	/// <param name="data">the data to print</param>
	public static void debugMarkup(params dynamic[] data) {
		if (!verbose) return;
		AnsiConsole.Markup("[cyan][[DEBUG]][/]: ");
		AnsiConsole.MarkupLine(string.Join(" ", data));
	}

	/// <summary>
	/// Same as info() but support for markup
	/// </summary>
	/// <param name="data">the data to print</param>
	public static void infoMarkup(params dynamic[] data) {
		AnsiConsole.MarkupLine(string.Join(" ", data));
	}

	/// <summary>
	/// Prints an exception only when verbose logging is enabled
	/// </summary>
	/// <param name="e">the exception</param>
	public static void exception(Exception e) {
		if (!verbose) return;
		AnsiConsole.WriteException(e, ExceptionFormats.Default);
	}

	internal static void pluginDebug(string id, params dynamic[] data) {
		if (!verbose) return;
		AnsiConsole.Markup($"[cyan][[{id}/DEBUG]][/]: ");
		AnsiConsole.WriteLine(string.Join(" ", data));
	}
	internal static void pluginInfo(string id, params dynamic[] data) {
		AnsiConsole.Markup($"[cyan][[{id}]][/]: ");
		AnsiConsole.WriteLine(string.Join(" ", data));
	}

	internal static void pluginMarkupDebug(string id, params dynamic[] data) {
		if (!verbose) return;
		AnsiConsole.Markup($"[cyan][[{id}/DEBUG]][/]: ");
		AnsiConsole.MarkupLine(string.Join(" ", data));
	}
	internal static void pluginMarkupInfo(string id, params dynamic[] data) {
		AnsiConsole.Markup($"[cyan][[{id}]][/]: ");
		AnsiConsole.MarkupLine(string.Join(" ", data));
	}
}
