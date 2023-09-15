
using System.Text.RegularExpressions;
using Manila.Core;
using Microsoft.ClearScript;

/// <summary>
/// An abstract class for project filtering.
/// </summary>
public abstract class ProjectFilter {
	private readonly ScriptObject func;

	/// <summary>
	/// Initializes a new instance of the ProjectFilter class with a dynamic function delegate.
	/// </summary>
	/// <param name="func">The dynamic function delegate to be associated with the filter.</param>
	public ProjectFilter(ScriptObject func) {
		this.func = func;
	}

	/// <summary>
	/// Runs the filter on the specified project and executes the associated dynamic function delegate if the project passes the filter.
	/// </summary>
	/// <param name="project">The project to be evaluated by the filter.</param>
	public void run(Project project) {
		if (predicate(project)) func.InvokeAsFunction();
	}

	/// <summary>
	/// Defines the predicate function for project filtering.
	/// </summary>
	/// <param name="project">The project to be evaluated by the filter.</param>
	/// <returns>True if the project passes the filter; otherwise, false.</returns>
	public abstract bool predicate(Project project);

	/// <summary>
	/// Represents a project filter based on regular expressions.
	/// </summary>
	public class RegexFilter : ProjectFilter {
		private Regex filter;

		/// <summary>
		/// Initializes a new instance of the RegexFilter class with the specified regular expression filter.
		/// </summary>
		/// <param name="filter">The regular expression pattern used for filtering.</param>
		public RegexFilter(Regex filter, ScriptObject func) : base(func) { this.filter = filter; }

		/// <inheritdoc />
		public override bool predicate(Project project) { return filter.IsMatch(project.name); }
	}

	/// <summary>
	/// Represents a project filter based on an array of string filters.
	/// </summary>
	public class ArrayFilter : ProjectFilter {
		private string[] filter;

		/// <summary>
		/// Initializes a new instance of the ArrayFilter class with the specified array of string filters.
		/// </summary>
		/// <param name="filter">The array of string filters used for matching.</param>
		public ArrayFilter(string[] filter, ScriptObject func) : base(func) { this.filter = filter; }

		/// <inheritdoc />
		public override bool predicate(Project project) { return filter.Contains(project.name); }
	}

	/// <summary>
	/// Represents a project filter based on a specific string filter.
	/// </summary>
	public class SpecificFilter : ProjectFilter {
		public string filter;

		/// <summary>
		/// Initializes a new instance of the SpecificFilter class with the specified string filter.
		/// </summary>
		/// <param name="filter">The specific string filter used for matching.</param>
		public SpecificFilter(string filter, ScriptObject func) : base(func) { this.filter = filter; }

		/// <inheritdoc />
		public override bool predicate(Project project) { return project.name == filter; }
	}
}

/// <summary>
/// A factory class for creating instances of ProjectFilter.
/// </summary>
public static class ProjectFilterFactory {
	/// <summary>
	/// Creates a ProjectFilter instance based on a regular expression filter.
	/// </summary>
	/// <param name="filter">The regular expression pattern used for filtering.</param>
	/// <returns>A ProjectFilter instance based on the regular expression filter.</returns>
	public static ProjectFilter create(Regex filter, ScriptObject func) { return new ProjectFilter.RegexFilter(filter, func); }

	/// <summary>
	/// Creates a ProjectFilter instance based on an array of string filters.
	/// </summary>
	/// <param name="filter">The array of string filters used for matching.</param>
	/// <returns>A ProjectFilter instance based on the array of string filters.</returns>
	public static ProjectFilter create(string[] filter, ScriptObject func) { return new ProjectFilter.ArrayFilter(filter, func); }

	/// <summary>
	/// Creates a ProjectFilter instance based on a specific string filter.
	/// </summary>
	/// <param name="filter">The specific string filter used for matching.</param>
	/// <returns>A ProjectFilter instance based on the specific string filter.</returns>
	public static ProjectFilter create(string filter, ScriptObject func) { return new ProjectFilter.SpecificFilter(filter, func); }
}
