
using Spectre.Console;

namespace Manila.Plugin.API;

public abstract class Plugin {
	public string name { get; }
	public string description { get; }
	public string version { get; }

	public Plugin(string name, string description, string version) {
		this.name = name;
		this.description = description;
		this.version = version;
	}

	public void print(params dynamic[] data) {
		print(true, data);
	}
	public void print(bool prefix, params dynamic[] data) {
		if (prefix) AnsiConsole.Markup($"[gray][[[/][cyan]{name}[/][gray]]]:[/] ");
		AnsiConsole.WriteLine(string.Join(" ", data));
	}

	public void markup(params dynamic[] data) {
		markup(true, data);
	}
	public void markup(bool prefix, params dynamic[] data) {
		if (prefix) AnsiConsole.Markup($"[gray][[[/][cyan]{name}[/][gray]]]:[/] ");
		AnsiConsole.MarkupLine(string.Join(" ", data));
	}

	public abstract void init();
}
