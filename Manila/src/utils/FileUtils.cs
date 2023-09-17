
using Manila.Plugin.API;
using Manila.Scripting.API;

namespace Manila.Utils;

public static class FileUtils {
	public static ManilaDirectory manilaDirectory { get; private set; }
	public static ManilaDirectory pluginsDirectory { get; private set; }

	public static ManilaFile pluginsFile { get; private set; }

	public static ManilaDirectory dataDirectory { get; private set; }

	public static ManilaFile workspaceFile { get; private set; }

	/// <summary>
	/// Initilizes the File Utilites
	/// </summary>
	/// <param name="rootDir">the working directory</param>
	public static void init(ManilaDirectory rootDir) {
		workspaceFile = rootDir.file("workspace.manila");

		manilaDirectory = rootDir.join(".manila");
		pluginsDirectory = manilaDirectory.join("plugins");
		dataDirectory = manilaDirectory.join("data");

		pluginsFile = manilaDirectory.file("plugins.manila");

		Logger.debug("Manila Dir:", manilaDirectory.getPath());
		Logger.debug("Plugins Dir:", pluginsDirectory.getPath());
		Logger.debug("Plugins File:", pluginsFile.getPath());
	}

	public static ManilaFile getStorage(Storage storage, Plugin.API.Plugin plugin) {
		var pluginData = dataDirectory.join(plugin.id);
		pluginData.create();

		return pluginData.file($"{storage.id}.json");
	}
}
