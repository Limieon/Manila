
using Manila.CLI;
using Manila.Core;
using Manila.Scripting;

namespace Manila.Commands;

internal class CommandBuild : Command {
	public CommandBuild() : base("build", "Build projects using buildfiles") {
	}


	public override void onExecute(Dictionary<string, object> p, Dictionary<string, object> o, App app) {
		ScriptManager.runWorkspaceFile();
		WorkspaceUtils.launchBuildConfigurators((string) o["toolset"]);
	}
}
