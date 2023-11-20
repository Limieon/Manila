
using Manila.CLI;
using Manila.Core;
using Manila.Scripting;

namespace Manila.Commands;

internal class CommandGenerate : Command {
	public CommandGenerate() : base("generate", "Generate buildfiles") {
		addOption(new Option("toolset", "overrides the toolset to use", "t", "msvc", Option.Type.STRING));
	}

	public override void onExecute(Dictionary<string, object> p, Dictionary<string, object> o, App app) {
		ScriptManager.runWorkspaceFile();
		WorkspaceUtils.launchGenerateConfigurators((string) o["toolset"]);
	}
}
