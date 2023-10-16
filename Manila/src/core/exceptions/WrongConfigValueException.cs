
namespace Manila.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a property of a build config has an invalid value
/// </summary>
public class WrongConfigValueException : Exception {
	/// <summary>
	/// Gets the config where the property was not found
	/// </summary>
	public readonly BuildConfig config;
	/// <summary>
	/// Gets the value that was provided
	/// </summary>
	public readonly string value;
	/// <summary>
	/// Gets the valid values the property could have
	/// </summary>
	public readonly string[] validValues;

	/// <summary>
	/// Initializes a new instance of the <see cref="PropertyNotFoundException"/> class
	/// with the specified project and property name
	/// </summary>
	/// <param name="value">The provided value</param>
	/// <param name="validValues">A array containing valid values</param>
	public WrongConfigValueException(string value, string[] validValues) {
		this.value = value;
		this.validValues = validValues;
	}
}
