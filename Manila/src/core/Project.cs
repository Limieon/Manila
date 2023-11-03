
using System.Collections.Immutable;
using Manila.Plugin.API;
using Manila.Scripting.API;
using Manila.Utils;
using Microsoft.ClearScript;

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
	/// The id of the project
	/// </summary>
	public readonly string id;

	/// <summary>
	/// Gets the name of the project
	/// </summary>
	public string name { get; set; }

	/// <summary>
	/// Gets the version of the project
	/// </summary>
	public string version { get; set; }

	/// <summary>
	/// Gets the location of the project
	/// </summary>
	public ManilaDirectory location { get; }

	/// <summary>
	/// Gets the associated workspace for the project
	/// </summary>
	public Workspace workspace { get; }

	/// <summary>
	/// The binary built by this project
	/// </summary>
	public ManilaFile? binary;

	public ProjectConfigurator configurator { get; set; }

	/// <summary>
	/// A random unique identifier
	/// </summary>
	public Guid uuid = Guid.NewGuid();

	public Scripting.API.Task getBuildTask() {
		foreach (var t in ScriptManager.getTasks("manila/build")) {
			if (t.project.id == id) return t;
		}
		throw new Exception($"Project '{id}' does not contain a build task!");
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Project"/> class
	/// </summary>
	/// <param name="id">The id of the project</param>
	/// <param name="location">The location of the project</param>
	/// <param name="workspace">The associated workspace for the project</param>
	public Project(string id, ManilaDirectory location, Workspace workspace) : base() {
		name = "";
		version = "";
		this.id = id;
		this.location = location;
		this.workspace = workspace;
	}

	/// <summary>
	/// Adds properties to the project
	/// </summary>
	/// <param name="properties">The properties</param>
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

	public ManilaFile getBinary() {
		if (binary == null) throw new Exception("Binary has not been built!");
		return binary;
	}

	public void configure(ScriptObject func) {
		func.InvokeAsFunction(configurator);
	}
	public void applyConfigurator() {
		configurator.check();
		properties = properties.Concat(configurator.getProperties().Where(x => !properties.ContainsKey(x.Key))).ToDictionary(x => x.Key, x => x.Value);
	}
}
