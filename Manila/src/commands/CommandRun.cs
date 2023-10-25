
using Manila.Core;
using Manila.Utils;

namespace Manila.Commands;

internal class CommandRun : CLI.Command {
	public CommandRun() : base("run", "Run a Manila project") {
	}

	public override async void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
		Console.WriteLine("Running...");

		try {
			var tasks = ScriptManager.getTasks("manila/run");

			foreach (var t in tasks) {
				await ScriptUtils.executeTask(t);
			}
		} catch (Exception e) {
			Logger.infoMarkup($"[red]{e.Message}[/]");
			Logger.exception(e);
		}
	}
}
