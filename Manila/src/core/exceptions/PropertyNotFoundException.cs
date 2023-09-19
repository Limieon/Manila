
namespace Manila.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a property is not found in a project
/// </summary>
public class PropertyNotFoundException : Exception {
	/// <summary>
	/// Gets the project where the property was not found
	/// </summary>
	public readonly Project project;

	/// <summary>
	/// Gets the name of the property that was not found
	/// </summary>
	public readonly string property;

	/// <summary>
	/// Initializes a new instance of the <see cref="PropertyNotFoundException"/> class
	/// with the specified project and property name
	/// </summary>
	/// <param name="project">The project where the property was not found</param>
	/// <param name="property">The name of the property that was not found</param>
	public PropertyNotFoundException(Project project, string property) {
		this.project = project;
		this.property = property;
	}
}
