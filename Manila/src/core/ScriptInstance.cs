
namespace Manila.Core;

/// <summary>
/// Represents a ScriptInstance that allows the addition and retrieval of custom properties.
/// </summary>
public class SriptInstance {
	private readonly Dictionary<string, dynamic> properties;

	/// <summary>
	/// Adds custom properties to the project.
	/// </summary>
	/// <param name="properties">A dictionary of key-value pairs representing the properties to add.</param>
	public void addProperties(Dictionary<string, dynamic> properties) {
		foreach (var e in properties) { properties[e.Key] = e.Value; }
	}

	/// <summary>
	/// Gets the value of a custom property by its key.
	/// </summary>
	/// <param name="key">The key of the property to retrieve.</param>
	/// <returns>The value associated with the specified key.</returns>
	public dynamic getProperty(string key) { return properties[key]; }
}
