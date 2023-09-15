
using Manila.Scripting.API;

namespace Manila.Core;

/// <summary>
/// Represents a workspace containing projects and project filters.
/// </summary>
/// <remarks>
/// A workspace is a container for organizing projects and applying project filters.
/// </remarks>
public class Workspace {
	/// <summary>
	/// Gets the name of the workspace.
	/// </summary>
	public string name { get; }

	/// <summary>
	/// Gets the location of the workspace.
	/// </summary>
	public ManilaDirectory location { get; }

	/// <summary>
	/// Gets the list of projects in the workspace.
	/// </summary>
	public List<Project> projects { get; }

	private readonly List<ProjectFilter> projectFilters;

	/// <summary>
	/// Initializes a new instance of the <see cref="Workspace"/> class.
	/// </summary>
	/// <param name="name">The name of the workspace.</param>
	/// <param name="location">The location of the workspace.</param>
	public Workspace(string name, ManilaDirectory location) {
		this.name = name;
		this.location = location;

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
	/// Runs all project filters on the projects in the workspace.
	/// </summary>
	public void runFilters() {
		foreach (var f in projectFilters) { foreach (var p in projects) { f.run(p); } }
	}
}
