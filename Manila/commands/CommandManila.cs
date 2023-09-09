
namespace Manila.Commands {
	public class CommandManila : CLI.Command {
		public CommandManila() : base("init", "initializes a new manila project") { }

		public override void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
			Console.WriteLine("Main Command!");
		}
	}
}
