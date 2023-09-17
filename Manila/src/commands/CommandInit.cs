
namespace Manila.Commands;

internal class CommandInit : CLI.Command {
	public CommandInit() : base("init", "Initialize a new manila project") {
	}

	public override void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
		Console.WriteLine("Executing Init...");
		var worksapce = new Data.Workspace();
		worksapce.write();
	}
}
