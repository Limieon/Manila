
using Manila.Scripting;

namespace Manila.Commands {
	class CommandManila : CLI.Command {
		public CommandManila() : base("init", "initializes a new manila project") { }

		public override void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
			new ScriptEngine();
		}
	}
}
