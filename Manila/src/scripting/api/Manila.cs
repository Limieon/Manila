
namespace Manila.Scripting.API {
	/// <summary>
	/// The main Manila Build System API
	/// </summary>
	public static class Manila {
		/// <summary>
		/// Creates a new file handle
		/// </summary>
		/// <param name="path">the path to the file</param>
		/// <returns>the file handle</returns>
		public static ManilaFile file(params string[] path) {
			return new ManilaFile(path);
		}
		/// <summary>
		/// Copies a file handle
		/// </summary>
		/// <param name="file">the original file handle</param>
		/// <returns></returns>
		public static ManilaFile file(ManilaFile file) {
			return new ManilaFile(file.path);
		}

		/// <summary>
		/// Creates a new directory handle
		/// </summary>
		/// <param name="path">the path to the directory</param>
		/// <returns>the directory handle</returns>
		public static ManilaDirectory dir(params string[] path) {
			return new ManilaDirectory(path);
		}
		/// <summary>
		/// Creates a new directory handle
		/// </summary>
		/// <param name="path">the path to the directory</param>
		/// <returns>the directory handle</returns>
		public static ManilaDirectory directory(params string[] path) {
			return new ManilaDirectory(path);
		}
		/// <summary>
		/// Copies a directory handle
		/// </summary>
		/// <param name="dir">the original directory handle</param>
		/// <returns>the directory handle</returns>
		public static ManilaDirectory dir(ManilaDirectory dir) {
			return new ManilaDirectory(dir.path);
		}
		/// <summary>
		/// Creates a new directory handle
		/// </summary>
		/// <param name="dir">the original directory handle</param>
		/// <returns>the directory handle</returns>
		public static ManilaDirectory directory(ManilaDirectory dir) {
			return new ManilaDirectory(dir.path);
		}
	}
}
