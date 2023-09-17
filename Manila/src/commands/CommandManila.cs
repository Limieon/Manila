
using Manila.CLI;
using Manila.Scripting;

namespace Manila.Commands;

internal class CommandManila : CLI.Command {
	public CommandManila() : base("init", "initializes a new manila project") { }

	public override void onExecute(Dictionary<string, object> p, Dictionary<string, object> o, App app) {
		app.runHelpCommand();
	}
}
