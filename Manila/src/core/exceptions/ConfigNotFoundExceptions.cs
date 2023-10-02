
namespace Manila.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a property is not found in a project
/// </summary>
public class ConfigNotFoundException : Exception {
	/// <summary>
	/// Gets the config name that was not found
	/// </summary>
	public readonly string name;

	/// <summary>
	/// The type where the config was searched on
	/// </summary>
	public readonly Type type;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConfigNotFoundException"/> class
	/// </summary>
	/// <param name="name">The name of the field that was not found</param>
	/// <param name="type">The type where the config was searched on</param>
	public ConfigNotFoundException(string name, Type type) : base($"Field '{name}' could not be found on '{type.FullName}'") {
		this.name = name;
		this.type = type;
	}
}
