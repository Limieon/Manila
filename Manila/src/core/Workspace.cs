
using System.Reflection.Metadata.Ecma335;
using Manila.Scripting.API;
using Manila.Utils;

namespace Manila.Core;

/// <summary>
/// Represents a workspace containing projects and project filters.
/// </summary>
/// <remarks>
/// A workspace is a container for organizing projects and applying project filters.
/// </remarks>
public class Workspace : ScriptInstance {
	/// <summary>
	/// Gets the name of the workspace.
	/// </summary>
	public string name { get; set; }

	/// <summary>
	/// Gets the location of the workspace.
	/// </summary>
	public ManilaDirectory location { get; }

	/// <summary>
	/// Gets the list of projects in the workspace.
	/// </summary>
	public List<Project> projects { get; }

	private readonly List<ProjectFilter> projectFilters;

	public readonly List<string> configurations;

	/// <summary>
	/// Gets named directories defined by the workspace
	/// </summary>
	public readonly Dictionary<string, ManilaDirectory> namedDirectories = new Dictionary<string, ManilaDirectory>();

	/// <summary>
	/// Initializes a new instance of the <see cref="Workspace"/> class.
	/// </summary>
	/// <param name="location">The location of the workspace.</param>
	public Workspace(ManilaDirectory location, List<string> configurations) {
		name = "";
		this.location = location;
		this.configurations = configurations;

		projects = new List<Project>();
		projectFilters = new List<ProjectFilter>();
	}

	/// <summary>
	/// Adds a project to the workspace.
	/// </summary>
	/// <param name="project">The project to add.</param>
	public void addProject(Project project) {
		projects.Add(project);
	}

	/// <summary>
	/// Adds a project filter to the workspace.
	/// </summary>
	/// <param name="filter">The project filter to add.</param>
	public void addProjectFilter(ProjectFilter filter) {
		projectFilters.Add(filter);
	}

	/// <summary>
	/// Sorts the project filters
	/// </summary>
	public void cookFilters() {
		projectFilters.Sort((a, b) => {
			var pa = ProjectFilter.getPriority(a);
			var pb = ProjectFilter.getPriority(b);

			if (pa == pb) return a.id - b.id;
			return pb - pa;
		});
	}

	/// <summary>
	/// Runs all project filters on the projects in the workspace.
	/// </summary>
	public void runFilters(Project p) {
		foreach (var f in projectFilters) f.run(p);
	}

	public Project getProject(string project) {
		foreach (var p in projects) if (p.id == project) return p;
		throw new ArgumentException($"Cannot find registered project with id '{project}'!");
	}

	/// <summary>
	/// Adds properties to a workspace
	/// </summary>
	/// <param name="properties">The properties to add</param>
	public override void addProperties(Dictionary<string, dynamic> properties) {
		foreach (var e in properties) {
			switch (e.Key) {
				case "name":
					name = e.Value;
					break;
			}
		}

		base.addProperties(properties);
	}
}
