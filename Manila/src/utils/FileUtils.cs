
using Manila.Plugin.API;
using Manila.Scripting.API;

namespace Manila.Utils;

/// <summary>
/// Provides utility functions to work with files inside the working directory
/// </summary>
public static class FileUtils {
	/// <summary>
	/// The .manila dir inside the working directory
	/// </summary>
	public static ManilaDirectory manilaDirectory { get; private set; }
	/// <summary>
	/// The .manila/plugins dir inside the working directory
	/// </summary>
	public static ManilaDirectory pluginsDirectory { get; private set; }

	/// <summary>
	/// The .manila/plugins.manila file inside the working directory
	/// </summary>
	public static ManilaFile pluginsFile { get; private set; }

	/// <summary>
	/// The .manila/data dir inside the working directory
	/// </summary>
	public static ManilaDirectory dataDirectory { get; private set; }

	/// <summary>
	/// The workspace.manila file inside the working directory
	/// </summary>
	public static ManilaFile workspaceFile { get; private set; }

	/// <summary>
	/// Initilizes the File Utilites
	/// </summary>
	/// <param name="rootDir">the working directory</param>
	public static void init(ManilaDirectory? rootDir, bool create) {
		if (rootDir == null) rootDir = Scripting.API.Manila.dir(Directory.GetCurrentDirectory());

		workspaceFile = rootDir.file("workspace.manila");

		manilaDirectory = rootDir.join(".manila");
		pluginsDirectory = manilaDirectory.join("plugins");
		dataDirectory = manilaDirectory.join("data");

		pluginsFile = manilaDirectory.file("plugins.manila");

		Logger.debug("--- FileUtils Info ---");
		Logger.debug("Workspace File:", workspaceFile.getPath());
		Logger.debug("Manila Dir:", manilaDirectory.getPath());
		Logger.debug("Plugins Dir:", pluginsDirectory.getPath());
		Logger.debug("Data Dir:", dataDirectory.getPath());
		Logger.debug("Plugins File:", pluginsFile.getPath());
		Logger.debug("--- FileUtils Info ---");

		if (create) {
			manilaDirectory.create();
			pluginsDirectory.create();
			dataDirectory.create();
		}
	}

	/// <summary>
	/// Gets the storage file associated to a storage inside a plugin
	/// </summary>
	/// <param name="storage">The storage</param>
	/// <param name="plugin">The plugin</param>
	public static ManilaFile getStorage(Storage storage, Plugin.API.Plugin plugin) {
		var pluginData = dataDirectory.join(plugin.id);
		pluginData.create();

		return pluginData.file($"{storage.id}.json");
	}
}
