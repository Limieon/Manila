
using Manila.Plugin.API;
using Manila.Utils;

namespace Manila.Scripting;

public static class WorkspaceUtils {
	public static void launchGenerateConfigurators(string? toolset) {
		var configurators = new List<ProjectConfigurator>();
		foreach (var prj in API.Manila.getWorkspace().projects) {
			if (!configurators.Contains(prj.configurator)) configurators.Add(prj.configurator);
		}

		Logger.debug($"Generating using {configurators.Count} configurator(s)...");
		foreach (var cfg in configurators) {
			cfg.generate(API.Manila.getWorkspace(), toolset == null ? API.Manila.getWorkspace().getProperty("toolset") : toolset);
		}
	}

	public static void launchBuildConfigurators(string? toolset) {
		var configurators = new List<ProjectConfigurator>();
		foreach (var prj in API.Manila.getWorkspace().projects) {
			if (!configurators.Contains(prj.configurator)) configurators.Add(prj.configurator);
		}

		Logger.debug($"Building using {configurators.Count} configurator(s)...");
		foreach (var cfg in configurators) {
			cfg.build(API.Manila.getWorkspace(), toolset == null ? API.Manila.getWorkspace().getProperty("toolset") : toolset);
		}
	}
}
