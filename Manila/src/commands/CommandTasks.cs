
using Manila.CLI;
using Manila.Core;
using Manila.Utils;
using Spectre.Console;

public class CommandTasks : Command {
	public CommandTasks() : base("tasks", "Prints available tasks") {
	}

	public override async void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
		var table = new Table().AddColumn("[blue]ID[/]").AddColumn("[blue]Project[/]").AddColumn("[blue]Tags[/]");
		var tasks = ScriptManager.getTasksAsMap();

		Logger.infoMarkup($"[gray]Found[/] [yellow]{ScriptManager.getTasks().Count}[/] [cyan]Task(s)[/] [gray]in[/] [yellow]{tasks.Keys.Count}[/] [magenta]Project(s)[/]");
		foreach (var k in tasks.Keys) {
			foreach (var t in tasks[k]) {
				table.AddRow(
					ScriptUtils.getTaskName(t)
						.Replace(t.name[1..], $"[cyan]{t.name[1..]}[/]")
						.Replace(t.project.id, $"[magenta]{t.project.id}[/]")
						.Replace(":", "[gray]:[/]"),

						t.project.name,
						t.tags.Count > 0 ? string.Join("[gray],[/] ", t.tags) : ""
				);
			}
		}
		table.Border(TableBorder.Rounded);
		AnsiConsole.Write(table);
	}
}
