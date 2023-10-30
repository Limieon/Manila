
using Manila.Core;

namespace Manila.Plugin.API;

public abstract class ProjectConfigurator {
	public abstract void init();
	public abstract Dictionary<string, dynamic> getProperties();
}
