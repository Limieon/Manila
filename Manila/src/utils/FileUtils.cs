
using Manila.Scripting.API;

namespace Manila.Utils;

public static class FileUtils {
	public static ManilaDirectory manilaDirectory { get; private set; }
	public static ManilaDirectory pluginsDirectory { get; private set; }

	public static ManilaFile pluginsFile { get; private set; }

	/// <summary>
	/// Initilizes the File Utilites
	/// </summary>
	/// <param name="rootDir">the working directory</param>
	public static void init(ManilaDirectory rootDir) {
		manilaDirectory = rootDir.join(".manila");
		pluginsDirectory = manilaDirectory.join("plugins");
		pluginsFile = manilaDirectory.file("plugins.manila");

		manilaDirectory.create();

		Logger.debug("Manila Dir:", manilaDirectory.getPath());
		Logger.debug("Plugins Dir:", pluginsDirectory.getPath());
		Logger.debug("Plugins File:", pluginsFile.getPath());
	}
}
