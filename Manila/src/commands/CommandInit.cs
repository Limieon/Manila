
using Manila.Scripting.API;
using Manila.Utils;
using Spectre.Console;

namespace Manila.Commands;

internal class CommandInit : CLI.Command {
	public CommandInit() : base("init", "Initialize a new manila workspace") {
		addParameter(new CLI.Parameter("name", "the workspace name", CLI.Parameter.Type.STRING));
	}

	public override void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
		var name = (string) p["name"];

		var dir = Path.Join(Directory.GetCurrentDirectory(), name);
		if (Directory.Exists(dir)) {
			if (!AnsiConsole.Confirm("[yellow]Directory already exists![/] [red]Override?[/]", false)) return;
		}

		AnsiConsole.MarkupLine($"Generating project [blue]{name}[/]...");
		var generator = new ProjectGenerator(name);
		generator.generateWorkspaceFile();
		AnsiConsole.MarkupLine("[green]Done![/]");
	}
}