
using Manila.Core;
using Manila.Plugin.API;
using Manila.Scripting.API;

namespace Manila.Scripting;

public static class WorkspaceUtils {
	public static void launchGenerateConfigurators(string? toolset) {
		var configurators = new List<ProjectConfigurator>();
		foreach (var prj in API.Manila.getWorkspace().projects) {
			if (!configurators.Contains(prj.configurator)) configurators.Add(prj.configurator);
		}

		foreach (var cfg in configurators) {
			cfg.generate(API.Manila.getWorkspace(), toolset == null ? API.Manila.getWorkspace().getProperty("toolset") : toolset);
		}
	}

	public static void launchBuildConfigurators(string? toolset) {
		var configurators = new List<ProjectConfigurator>();
		foreach (var prj in API.Manila.getWorkspace().projects) {
			if (!configurators.Contains(prj.configurator)) configurators.Add(prj.configurator);
		}

		foreach (var cfg in configurators) {
			cfg.build(API.Manila.getWorkspace(), toolset == null ? API.Manila.getWorkspace().getProperty("toolset") : toolset);
		}
	}
}
