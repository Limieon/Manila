
using System.Collections.Immutable;
using Manila.Scripting.API;

namespace Manila.Core;

/// <summary>
/// Represents a project in the Manila buildsystem.
/// </summary>
/// <remarks>
/// A project is defined by its name, version, location, and associated workspace.
/// It also allows the addition and retrieval of custom properties.
/// </remarks>
public class Project : ScriptInstance {
	/// <summary>
	/// Gets the name of the project.
	/// </summary>
	public string name { get; set; }

	/// <summary>
	/// Gets the version of the project.
	/// </summary>
	public string version { get; set; }

	/// <summary>
	/// Gets the location of the project.
	/// </summary>
	public ManilaDirectory location { get; }

	/// <summary>
	/// Gets the associated workspace for the project.
	/// </summary>
	public Workspace workspace { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Project"/> class.
	/// </summary>
	/// <param name="name">The name of the project.</param>
	/// <param name="version">The version of the project.</param>
	/// <param name="location">The location of the project.</param>
	/// <param name="workspace">The associated workspace for the project.</param>
	public Project(ManilaDirectory location, Workspace workspace) : base() {
		name = "";
		version = "";
		this.location = location;
		this.workspace = workspace;
	}

	public override void addProperties(Dictionary<string, dynamic> properties) {
		foreach (var e in properties) {
			switch (e.Key) {
				case "name":
					name = e.Value;
					break;
				case "version":
					version = e.Value;
					break;
			}
		}

		base.addProperties(properties);
	}
}
