
using Manila.Utils;

namespace Manila.Core;

/// <summary>
/// Represents a ScriptInstance that allows the addition and retrieval of custom properties.
/// </summary>
public class ScriptInstance {
	private readonly Dictionary<string, dynamic> properties = new Dictionary<string, dynamic>();

	/// <summary>
	/// Adds custom properties to the project.
	/// </summary>
	/// <param name="properties">A dictionary of key-value pairs representing the properties to add.</param>
	public virtual void addProperties(Dictionary<string, dynamic> properties) {
		foreach (var e in properties) { this.properties[e.Key] = e.Value; }
	}

	/// <summary>
	/// Gets the value of a custom property by its key.
	/// </summary>
	/// <param name="key">The key of the property to retrieve.</param>
	/// <returns>The value associated with the specified key.</returns>
	public dynamic getProperty(string key) {
		if (!properties.ContainsKey(key)) throw new KeyNotFoundException("Object does not contain property '" + key + "'!");
		return properties[key];
	}
}
