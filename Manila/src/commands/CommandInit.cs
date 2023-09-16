
namespace Manila.Commands;

internal class CommandInit : CLI.Command {
	public CommandInit() : base("init", "Initializes a new manila project") {
		addParameter(new CLI.Parameter("input", "the input", CLI.Parameter.Type.STRING));
		addParameter(new CLI.Parameter("output", "the output", CLI.Parameter.Type.STRING));
		addParameter(new CLI.Parameter("amount", "the amount", CLI.Parameter.Type.NUMBER));

		addOption(new CLI.Option("dir", "changes working directory", ".", "d", CLI.Option.Type.STRING));
		addOption(new CLI.Option("repeat", "how many times to repeat", 5, "r", CLI.Option.Type.NUMBER));
		addOption(new CLI.Option("verbose", "enable verbose logging", false, "v", CLI.Option.Type.FLAG));
	}

	public override void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
		Console.WriteLine("Executing Init...");
		string input = (string) p["input"];
		string output = (string) p["output"];
		int amount = (int) p["amount"];

		Console.WriteLine("input: " + input);
		Console.WriteLine("output: " + output);
		Console.WriteLine("amount: " + amount);

		Console.WriteLine("Options:");
		foreach (KeyValuePair<string, object> entry in o) {
			Console.WriteLine(entry.Key + ": " + entry.Value);
		}
	}
}
