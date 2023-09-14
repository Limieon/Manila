
using Manila.Scripting;

using Spectre.Console;

namespace Manila.Plugin.API;

public abstract class Plugin {
	public string id { get; }
	public PluginManager.Meta meta { get; }

	public Plugin(string id) {
		this.id = id;
		var meta = PluginManager.getMeta(id);
		this.meta = meta;
	}

	public void print(params dynamic[] data) {
		print(true, data);
	}
	public void print(bool prefix, params dynamic[] data) {
		if (prefix) AnsiConsole.Markup($"[gray][[[/][cyan]{id}[/][gray]]]:[/] ");
		AnsiConsole.WriteLine(string.Join(" ", data));
	}

	public void markup(params dynamic[] data) {
		markup(true, data);
	}
	public void markup(bool prefix, params dynamic[] data) {
		if (prefix) AnsiConsole.Markup($"[gray][[[/][cyan]{id}[/][gray]]]:[/] ");
		AnsiConsole.MarkupLine(string.Join(" ", data));
	}

	public void addObject(string name, object obj) {
		ScriptEngine.getInstance().addObject(name, obj);
	}
	public void addType(Type type) {
		ScriptEngine.getInstance().addType(type);
	}

	public abstract void init();
	public abstract void shutdown();
}
