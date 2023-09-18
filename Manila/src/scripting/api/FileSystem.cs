
using Newtonsoft.Json;
using Microsoft.ClearScript;

namespace Manila.Scripting.API;

/// <summary>
/// A file handle
/// </summary>
public class ManilaFile {
	/// <summary>
	/// The absolute path this handle is pointing to
	/// </summary>
	public string path { get; }

	public ManilaFile(params string[] path) {
		if (path.Length < 1) throw new ArgumentException("Argument 'path' must contain at least one valid path!");
		var inPath = Path.IsPathRooted(path[0]) ? "" : Directory.GetCurrentDirectory();
		foreach (var p in path) {
			inPath = Path.Join(inPath, p);
		}

		if (Path.IsPathRooted(inPath)) this.path = inPath;
		else this.path = Path.Join(Directory.GetCurrentDirectory(), inPath);

		if (File.Exists(this.path) && File.GetAttributes(this.path) == FileAttributes.Directory) throw new ArgumentException("The path '" + this.path + "' is a directory!");
	}
	public ManilaFile(ManilaDirectory dir, string file) {
		path = Path.Join(dir.getPath(), file);

	}

	/// <summary>
	/// Writes a string to a file
	/// </summary>
	public ManilaFile write(string data) {
		string dir = Path.GetDirectoryName(path);
		if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
		File.WriteAllText(path, data);
		return this;
	}
	/// <summary>
	/// Writes a string array to a file
	/// </summary>
	public ManilaFile write(params string[] data) {
		string dir = Path.GetDirectoryName(path);
		if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
		File.WriteAllText(path, string.Join("\n", data));
		return this;
	}
	/// <summary>
	/// Writes a dynamic type to a file
	/// </summary>
	public ManilaFile write(dynamic data) {
		string dir = Path.GetDirectoryName(path);
		if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
		File.WriteAllText(path, string.Join("\n", data));
		return this;
	}

	/// <summary>
	/// Deletes a file (not the handle)
	/// </summary>
	public ManilaFile delete() {
		string dir = Path.GetDirectoryName(path);
		if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
		if (File.Exists(path)) File.Delete(path);
		return this;
	}

	/// <summary>
	/// Checks if a file exists
	/// </summary>
	public bool exists() {
		return File.Exists(path);
	}
	/// <summary>
	/// Reads the file as a string
	/// </summary>
	public string read() {
		return File.ReadAllText(path);
	}

	/// <summary>
	/// Deserializes a json file to an object
	/// </summary>
	/// <typeparam name="T">the type of the object</typeparam>
	/// <returns>the value of the object</returns>
	/// <exception cref="FileNotFoundException"></exception>
	public T deserializeJSON<T>() where T : new() {
		if (!exists()) throw new FileNotFoundException("File '" + getPath() + "' does not exist!");
		T? res = JsonConvert.DeserializeObject<T>(read());
		if (res == null) return new T();
		return res;
	}
	/// <summary>
	/// Serializes an object as a JSON file
	/// </summary>
	/// <param name="data">the object to serialize</param>
	/// <param name="format">if the json file should be formatted</param>
	public ManilaFile serializeJSON(object data, bool format = false) {
		write(JsonConvert.SerializeObject(data, format ? Formatting.Indented : Formatting.None));
		return this;
	}

	/// <summary>
	/// Renames a file
	/// </summary>
	/// <param name="name">new name</param>
	public ManilaFile rename(string name) {
		File.Move(path, name);
		return new ManilaFile(Path.Join(Directory.GetCurrentDirectory(), name));
	}
	/// <summary>
	/// Moves a file
	/// </summary>
	/// <param name="destination">the destination</param>
	public ManilaFile move(string destination) {
		if (Path.IsPathFullyQualified(destination)) {
			if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
			File.Move(path, destination);
			return this;
		}

		string newPath = Path.Join(Directory.GetCurrentDirectory(), destination, Path.GetFileName(path));
		if (!Directory.Exists(Path.GetDirectoryName(newPath))) Directory.CreateDirectory(Path.GetDirectoryName(newPath));
		File.Move(path, newPath);
		return new ManilaFile(newPath);
	}
	/// <summary>
	/// Copies a file to a new destinaton
	/// </summary>
	/// <param name="destination">the copy destination</param>
	/// <returns>the unchanged file handle</returns>
	public ManilaFile copy(string destination) {
		if (Path.IsPathRooted(destination)) {
			if (Directory.Exists(destination) && File.GetAttributes(destination) == FileAttributes.Directory) {
				File.Copy(path, Path.Join(destination, Path.GetFileName(path)));
			} else {
				File.Copy(path, destination, true);
			}
		} else {
			File.Copy(path, Path.Join(Directory.GetCurrentDirectory(), destination, Path.GetFileName(path)));
		}
		return this;
	}

	/// <summary>
	/// Gets the full parsed file path (will remove navigators like ../)
	/// </summary>
	public string getPath() { return Path.GetFullPath(path); }
	/// <summary>
	/// Gets the relative path from the current path to the specified destination path
	/// </summary>
	/// <param name="to">the destination path (default is current directory)</param>
	public string getPathRelative(string to = ".") { return Path.GetRelativePath(to, path); }
	/// <summary>
	/// Gets the name of the targeted file
	/// </summary>
	public string getFileName() { return Path.GetFileName(path); }
	/// <summary>
	/// Gets the path where the file is located in
	/// </summary>
	public string getFileDir() { return Path.GetDirectoryName(path); }
	public ManilaDirectory getFileDirHandle() { return new ManilaDirectory(getFileDir()); }

	public ManilaFile setExtension(string e) {
		return new ManilaFile(Path.Join(Path.GetDirectoryName(path), $"{Path.GetFileNameWithoutExtension(path)}.{e}"));
	}
}

/// <summary>
/// A directory hanlde
/// </summary>
public class ManilaDirectory {
	/// <summary>
	/// The absolute path this handle is pointing to
	/// </summary>
	public string path { get; }

	public ManilaDirectory(params string[] path) {
		if (path.Length < 1) throw new ArgumentException("Argument 'path' must contain at least one valid path!");
		var inPath = Path.IsPathRooted(path[0]) ? "" : Directory.GetCurrentDirectory();
		foreach (var p in path) {
			inPath = Path.Join(inPath, p);
		}

		this.path = inPath;
	}
	public ManilaDirectory(ManilaDirectory root, params string[] path) : this(Path.Join(root.getPath(), string.Join(Path.PathSeparator, path))) {
	}

	/// <summary>
	/// Joins a path to the current path
	/// </summary>
	/// <param name="paths">the paths to join</param>
	public ManilaDirectory join(params string[] paths) {
		var temp = path;
		foreach (var p in paths) {
			temp = Path.Join(temp, p);
		}
		return new ManilaDirectory(temp);
	}

	/// <summary>
	/// Gets the list of files contained in a directory
	/// </summary>
	/// <param name="func">the function to filter the files files</param>
	/// <param name="deep">flag to indicate whether subdirectories should also be searched</param>
	public ManilaFile[] files(ScriptObject? func = null, bool deep = false) {
		List<ManilaFile> result = new List<ManilaFile>();

		if (deep) {
			var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
			foreach (var file in files) {
				if (func == null || (bool) func.InvokeAsFunction(file)) {
					result.Add(Manila.file(file));
				}
			}
		} else {
			var files = Directory.GetFiles(path);
			foreach (var file in files) {
				if (func == null || (bool) func.InvokeAsFunction(file)) {
					result.Add(Manila.file(file));
				}
			}
		}

		return result.ToArray();
	}

	/// <summary>
	/// Gets the list of files contained in a directory
	/// </summary>
	/// <param name="func">the function to filter the files files</param>
	/// <param name="deep">flag to indicate whether subdirectories should also be searched</param>
	public List<ManilaFile> files(bool deep, Func<string, bool> func) {
		List<ManilaFile> result = new List<ManilaFile>();

		if (deep) {
			var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
			foreach (var file in files) {
				if (func == null || func(file)) {
					result.Add(Manila.file(file));
				}
			}
		} else {
			var files = Directory.GetFiles(path);
			foreach (var file in files) {
				if (func == null || func(file)) {
					result.Add(Manila.file(file));
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Gets the list of files contained in a directory
	/// </summary>
	/// <param name="func">the function to filter the files files</param>
	/// <param name="deep">flag to indicate whether subdirectories should also be searched</param>
	public ManilaFile[] files(bool deep) {
		return files(null, deep);
	}

	/// <summary>
	/// Checks if the directory exists
	/// </summary>
	public bool exists() { return Directory.Exists(path); }

	/// <summary>
	/// Creates the directory if it does not exist
	/// </summary>
	public void create() { if (!exists()) Directory.CreateDirectory(path); }

	/// <summary>
	/// Gets the full parsed path (will remove navigators like ../)
	/// </summary>
	public string getPath() { return Path.GetFullPath(path); }
	/// <summary>
	/// Gets the relative path from the current path to the specified destination path
	/// </summary>
	/// <param name="to">the destination path (default is current directory)</param>
	public string getPathRelative(string to = ".") { return Path.GetRelativePath(to, path); }

	/// <summary>
	/// Gets a ManilaFile handle to the specified file
	/// </summary>
	/// <param name="files">files to search</param>
	/// <returns>the file handle from the found file (if none is found, it retruns the first)</returns>
	public ManilaFile file(params string[] files) {
		if (files.Length < 1) throw new ArgumentException("Argument 'files' requires at least one filename!");

		foreach (var f in files) {
			ManilaFile handle = Manila.file(getPath(), f);
			if (handle.exists()) return handle;
		}
		return Manila.file(getPath(), files[0]);
	}
}
