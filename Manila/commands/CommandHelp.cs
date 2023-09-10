
using Spectre.Console;

namespace Manila.Commands {
	class CommandHelp : CLI.CommandHelp {
		private string parseParameters(CLI.Command c) {
			string o = "";
			for (var i = 0; i < c.parameters.Count; ++i) {
				var p = c.parameters[i];
				if (i < c.parameters.Count - 1) {
					o += $"[gray]<[/][deepskyblue3_1]{p.name}[/][gray]>[/] ";
				} else {
					o += $"[gray]<[/][deepskyblue3_1]{p.name}[/][gray]>[/]";
				}
			}
			return o;
		}
		private void printLogo() {
			AnsiConsole.Write(new FigletText(FigletFont.Parse(CLI.Fonts.Doom.get()), "Manila").LeftJustified().Color(Color.DodgerBlue3));
		}

		public override void printHelp(CLI.App a) {
			printLogo();
			AnsiConsole.MarkupLine($"{a.description}\n");

			AnsiConsole.MarkupLine($"[purple]Usage:[/] {a.name.ToLower()} [gray][[[/][deepskyblue3_1]command[/][gray]]][/] [gray][[[/][deepskyblue3_1]options[/][gray]]][/]\n");

			AnsiConsole.MarkupLine($"[purple]Available Commands:[/]");
			var table = new Table().AddColumn("").AddColumn("");

			foreach (CLI.Command c in a.commands) {
				table.AddRow($"  [skyblue1]{c.name}[/] {parseParameters(c)}", c.description);
			}
			table.HideHeaders();
			table.Border(TableBorder.None);
			AnsiConsole.Write(table);
		}
		public override void printHelp(CLI.Command c) {
			printLogo();
			AnsiConsole.MarkupLine($"{c.description}\n");

			AnsiConsole.MarkupLine($"[purple]Usage:[/] {c.name} {parseParameters(c)} [gray][[[/][deepskyblue3_1]options[/][gray]]][/]\n");

			AnsiConsole.MarkupLine($"[purple]Required Parameters:[/]");
			var table = new Table().AddColumn("").AddColumn("");

			foreach (CLI.Parameter p in c.parameters) {
				table.AddRow($"  [skyblue1]{p.name}[/]", p.description);
			}
			table.AddRow("", "");
			table.AddRow("[purple]Available Options:[/]", "");

			foreach (KeyValuePair<string, CLI.Option> entry in c.options) {
				var o = entry.Value;
				var parameter = o.type == CLI.Option.Type.FLAG ? "" : o.type == CLI.Option.Type.STRING ? "[gray]<[/][deepskyblue3_1]string[/][gray]>[/]" : "[gray]<[/][deepskyblue3_1]number[/][gray]>[/]";
				if (o.alias != "") {
					table.AddRow($"  [gray]-[/][orange1]{o.alias}[/], [gray]--[/][orange1]{o.name}[/] {parameter}", o.description);

					continue;
				}
				table.AddRow($"  [gray]--[/][orange1]{o.name}[/] {parameter}", o.description);
			}

			table.HideHeaders();
			table.Border(TableBorder.None);
			AnsiConsole.Write(table);
		}
	}
}
