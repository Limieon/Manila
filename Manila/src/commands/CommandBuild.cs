
using Manila.Core;
using Manila.Scripting.API;
using Manila.Utils;
using Spectre.Console;

namespace Manila.Commands;

internal class CommandBuild : CLI.Command {
	public CommandBuild() : base("build", "Build a Manila project") {
	}

	public override async void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
		foreach (var t in ScriptManager.getTasks("manila/build")) {
			await ScriptUtils.executeTask(t);
		}
	}
}
