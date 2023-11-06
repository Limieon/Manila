
using Manila.Scripting.API;
using Manila.Utils;

namespace Manila.Data;

/// <summary>
/// Represents a workspace in the Manila application
/// </summary>
public class Workspace {
	/// <summary>
	/// Represents the data associated with the workspace
	/// </summary>
	public class Data {
		public class ProjectData {
			public string configurator;
		}

		/// <summary>
		/// Gets or sets the name of the workspace
		/// </summary>
		public string? name = null;

		/// <summary>
		/// Gets or sets the list of authors associated with the workspace
		/// </summary>
		public List<string> authors = new List<string>();

		/// <summary>
		/// Gets or sets the object of projects in the workspace
		/// </summary>
		public Dictionary<string, ProjectData> projects = new Dictionary<string, ProjectData>();

		/// <summary>
		/// Gets or sets the Git repository URL for the workspace
		/// </summary>
		public string gitRepo;

		/// <summary>
		/// Gets the named directories defined in the workspace
		/// </summary>
		public readonly Dictionary<string, string> namedDirectories = new Dictionary<string, string>();

		/// <summary>
		/// Gets the configurations
		/// </summary>
		public List<string> configurations;
	}

	/// <summary>
	/// Gets the data associated with the workspace
	/// </summary>
	public readonly Data data;

	/// <summary>
	/// Initializes a new instance of the <see cref="Workspace"/> class
	/// </summary>
	public Workspace() {
		var f = new ManilaFile("workspace.manila");
		if (!f.exists()) { data = new Data(); return; }
		data = f.deserializeJSON<Data>();
	}

	/// <summary>
	/// Writes the workspace data to a file
	/// </summary>
	public void write() {
		FileUtils.workspaceFile.serializeJSON(data, true);
	}
}
