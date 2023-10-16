
using Manila.CLI;
using Manila.Core;
using Manila.Plugin;
using Manila.Utils;
using Spectre.Console;

public class CommandPlugins : Command {
	public CommandPlugins() : base("plugins", "Prints installed and loaded plugins") {
	}

	public override async void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
		var table = new Table().AddColumn("[blue]ID[/]").AddColumn("[blue]Name[/]").AddColumn("[blue]Description[/]").AddColumn("[blue]Authors[/]").AddColumn("[blue]Version[/]");

		Logger.infoMarkup($"[gray]Found[/] [yellow]{PluginManager.plugins.Count}[/] [cyan]Plugin(s)[/]");
		foreach (var plugin in PluginManager.plugins) {
			var meta = PluginManager.getMeta(plugin.id);
			table.AddRow(plugin.id, meta.name, meta.description, string.Join("[gray], [/]", meta.authors), meta.version);
		}

		table.Border(TableBorder.Rounded);
		AnsiConsole.Write(table);
	}
}

