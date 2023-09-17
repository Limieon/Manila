
namespace Manila.Core.Exceptions;

public class PropertyNotFoundException : Exception {
	public readonly Project project;
	public readonly string property;

	public PropertyNotFoundException(Project project, string property) {
		this.project = project;
		this.property = property;
	}
}
