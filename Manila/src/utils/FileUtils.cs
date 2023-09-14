
using Manila.Scripting.API;

namespace Manila.Utils;

public static class FileUtils {
	public static ManilaDirectory manilaDirectory { get; private set; }

	/// <summary>
	/// Initilizes the File Utilites
	/// </summary>
	/// <param name="rootDir">the working directory</param>
	public static void init(ManilaDirectory rootDir) {
		manilaDirectory = rootDir.join(".manila");

		manilaDirectory.create();

		Logger.debug("Manila Dir:", manilaDirectory.getPath());
	}
}
