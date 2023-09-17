
using Manila.Scripting.API;
using Manila.Utils;
using Spectre.Console;

namespace Manila.Commands;

internal class CommandInit : CLI.Command {
	public CommandInit() : base("init", "Initialize a new manila workspace") {
		addParameter(new CLI.Parameter("name", "the workspace name", CLI.Parameter.Type.STRING));

		addOption(new CLI.Option("default", "skip questions and generate default workspace", "d"));
	}

	public override void onExecute(Dictionary<string, object> p, Dictionary<string, object> o) {
		var name = (string) p["name"];

		var skip = (bool) o["default"];

		string gitRepo = "";

		if (!skip) {
			if (FileUtils.workspaceFile.exists() && !AnsiConsole.Confirm("Directory already contains a workspace! Override?", false)) return;

			gitRepo = AnsiConsole.Prompt(new TextPrompt<string>("[grey][[Optional]][/] Enter is your git repo:").AllowEmpty());
		}

		generateWorkspaceFile(name, gitRepo);
	}

	private void generateWorkspaceFile(string name, string gitRepo) {
		var workspace = new Data.Workspace();
		workspace.data.name = name;
		workspace.data.gitRepo = gitRepo == string.Empty ? null : gitRepo;
		workspace.write();
	}
}
