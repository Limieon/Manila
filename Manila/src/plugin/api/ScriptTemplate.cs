
using Manila.Core;

namespace Manila.Plugin.API;

public class ScriptTemplate {
	public readonly string id;
	public readonly Plugin? plugin;

	internal ScriptTemplate() {
		id = "DEFAULT";
		plugin = null;
	}

	public ScriptTemplate(Plugin plugin, string id) {
		this.plugin = plugin;
		this.id = id;
	}

	public virtual Project getProject() { return (Project) ScriptManager.currentScriptInstance; }
	public virtual Workspace getWorkspace() { return ScriptManager.workspace; }
	public virtual BuildConfig getBuildConfig() { return ScriptManager.buildConfig; }
}
