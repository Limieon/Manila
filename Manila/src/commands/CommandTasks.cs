
using Manila.CLI;
using Manila.Core;
using Manila.Utils;
using Spectre.Console;

public class CommandTasks : Command {
	public CommandTasks() : base("tasks", "Prints available tasks") {
		addOption(new Option("all", "also shows hidden tasks", "a", false, Option.Type.FLAG));
	}

	public override async void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
		var all = (bool) o["all"];

		var table = new Table().AddColumn("[blue]ID[/]").AddColumn("[blue]Project[/]").AddColumn("[blue]Tags[/]");
		var tasks = ScriptManager.getTasksAsMap();

		Logger.infoMarkup($"[gray]Found[/] [yellow]{ScriptManager.getTasks().Count}[/] [cyan]Task(s)[/] [gray]in[/] [yellow]{tasks.Keys.Count}[/] [magenta]Project(s)[/]");
		foreach (var k in tasks.Keys) {
			foreach (var t in tasks[k]) {
				if (!all && t.hidden) continue;

				if (all && t.hidden) {
					table.AddRow(
					ScriptUtils.getTaskName(t)
						.Replace(t.name[1..], $"[cyan]{t.name[1..]}[/]")
						.Replace(t.project.id, $"[magenta]{t.project.id}[/]")
						.Replace(":", "[gray]:[/]") + " [gray](hidden)[/]",

						$"[yellow]{t.project.name}[/]",
						t.tags.Count > 0 ? "[green]" + string.Join("[gray],[/] ", t.tags) + "[/]" : ""
				);
				} else {
					table.AddRow(
					ScriptUtils.getTaskName(t)
						.Replace(t.name[1..], $"[cyan]{t.name[1..]}[/]")
						.Replace(t.project.id, $"[magenta]{t.project.id}[/]")
						.Replace(":", "[gray]:[/]"),

						$"[yellow]{t.project.name}[/]",
						t.tags.Count > 0 ? "[green]" + string.Join("[gray],[/] ", t.tags) + "[/]" : ""
				);
				}
			}
		}
		table.Border(TableBorder.Rounded);
		AnsiConsole.Write(table);
	}
}
