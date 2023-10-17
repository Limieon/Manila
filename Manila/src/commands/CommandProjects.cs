
using Manila.CLI;
using Manila.Core;
using Manila.Plugin;
using Manila.Utils;
using Spectre.Console;

public class CommandProjects : Command {
	public CommandProjects() : base("projects", "Prints installed and loaded plugins") {
	}

	public override async void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
		var table = new Table().AddColumn("[blue]ID[/]").AddColumn("[blue]Name[/]").AddColumn("[blue]Version[/]").AddColumn("[blue]Dependency Count[/]");

		Logger.infoMarkup($"[gray]Found[/] [yellow]{ScriptManager.workspace.projects.Count}[/] [cyan]Project(s)[/]");
		foreach (var prj in ScriptManager.workspace.projects) {
			table.AddRow(prj.id, prj.name, prj.version, prj.depndencyResolvers.Count + "");
		}

		table.Border(TableBorder.Rounded);
		AnsiConsole.Write(table);
	}
}

