
using Manila.Core;
using Manila.Scripting.API;
using Manila.Utils;
using Spectre.Console;

namespace Manila.Commands;

internal class CommandRebuild : CLI.Command {
	public CommandRebuild() : base("rebuild", "Rebuild a Manila project") {
	}

	public override async void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
		foreach (var t in ScriptManager.getTasks("manila/rebuild")) {
			try {
				await ScriptUtils.executeTask(t);
			} catch (Exception e) {
				Logger.infoMarkup($"[red]{e.Message}[/]");
				Logger.exception(e);
			}
		}
	}
}
