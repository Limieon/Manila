
using Manila.CLI;
using Manila.Utils;

namespace Manila.Commands;

internal class CommandManila : CLI.Command {
	public CommandManila() : base("manila", "The default manila command used to run scripts") {
		addOption(new Option("version", "prints the version", "v", null, Option.Type.FLAG));
	}

	public override void onExecute(Dictionary<string, object> p, Dictionary<string, object> o, App app) {
		var version = (bool) o["version"];

		if (version) {
			Logger.info($"Current Version: {app.version}");
			return;
		}

		app.runHelpCommand();
	}
}
