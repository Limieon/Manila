
namespace Manila.Commands {
	public class CommandInit : CLI.Command {
		public CommandInit() : base("init", "initializes a new manila project") {
			addParameter(new CLI.Parameter("input", "the input", CLI.Parameter.Type.STRING));
			addParameter(new CLI.Parameter("output", "the output", CLI.Parameter.Type.STRING));

			addOption(new CLI.Option("dir", "changes working directory", "d", CLI.Option.Type.STRING));
		}

		public override void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
			Console.WriteLine("Executing Init...");

			Console.WriteLine("Parameters:");
			foreach (KeyValuePair<string, object> entry in p) {
				Console.WriteLine(entry.Key + ": " + entry.Value);
			}

			Console.WriteLine("Options:");
			foreach (KeyValuePair<string, object> entry in o) {
				Console.WriteLine(entry.Key + ": " + entry.Value);
			}
		}
	}
}
