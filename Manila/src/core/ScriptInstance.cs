
using Manila.Utils;

namespace Manila.Core;

/// <summary>
/// Represents a ScriptInstance that allows the addition and retrieval of custom properties.
/// </summary>
public class ScriptInstance : EventSystem {
	/// <summary>
	/// Properties for this instance
	/// </summary>
	internal Dictionary<string, dynamic> properties = new Dictionary<string, dynamic>();
	public readonly List<Scripting.API.Dependency.Resolver> depndencyResolvers = new List<Scripting.API.Dependency.Resolver>();

	/// <summary>
	/// Adds custom properties to the project.
	/// </summary>
	/// <param name="properties">A dictionary of key-value pairs representing the properties to add.</param>
	public virtual void addProperties(Dictionary<string, dynamic> properties) {
		foreach (var e in properties) {
			this.properties[e.Key] = e.Value;
		}

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

	/// <summary>
	/// Registers a basic property on the project
	/// </summary>
	/// <param name="id">The id of the property (required to get the data)</param>
	/// <param name="type">The type of the property</param>
	/// <param name="functionName">Sets the name of the function</param>
	/// <exception cref="ArgumentException"></exception>
	public void registerProperty(string id, Type type, string? functionName = null) {
		if (functionName == null) functionName = id;
		var func = (dynamic data) => {
			if (data.GetType() != type) throw new ArgumentException($"Tried to assign value of type 'string' to property '{id}' which required type '{type.FullName}'!");

			properties[id] = data;
		};

		ScriptManager.engine.engine.AddHostObject(functionName, func);
	}

	public void registerPropertyFunction<T>(string funcName, Action<T> func) {
		ScriptManager.engine.engine.AddHostObject(funcName, func);
	}
}
