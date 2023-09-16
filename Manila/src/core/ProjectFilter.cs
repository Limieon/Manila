
using System.Text.RegularExpressions;
using Manila.Core;
using Microsoft.ClearScript;

namespace Manila.Core;

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
		public override bool predicate(Project project) { return filter.IsMatch(project.id); }
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
		public override bool predicate(Project project) { return filter.Contains(project.id); }
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
		public override bool predicate(Project project) { return project.id == filter; }
	}
}
