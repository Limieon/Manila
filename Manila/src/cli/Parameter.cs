
using Manila.CLI.Exceptions;

namespace Manila.CLI;

/// <summary>
/// A parameter in a cli command
/// </summary>
public class Parameter {
	/// <summary>
	/// The type of the parameter
	/// </summary>
	public enum Type {
		/// <summary>
		/// String type
		/// </summary>
		STRING,
		/// <summary>
		/// Number type
		/// </summary>
		NUMBER
	}

	/// <summary>
	/// Gets the type of the parameter
	/// </summary>
	public Type type { get; private set; }
	/// <summary>
	/// Gets the name of the parameter
	/// </summary>
	public string name { get; private set; }
	/// <summary>
	/// Gets the description of the parameter
	/// </summary>
	public string description { get; private set; }

	/// <summary>
	/// Parses a user proved string to the parameter value
	/// </summary>
	/// <param name="proivded">The string</param>
	/// <exception cref="ParameterProvidedWrongTypeException">Gets thrown when a wrong type has been provided</exception>
	public object parse(string proivded) {
		if (type == Type.STRING) return proivded;

		try {
			return int.Parse(proivded);
		} catch (Exception) {
			throw new ParameterProvidedWrongTypeException(this, proivded);
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Parameter"/> class
	/// </summary>
	/// <param name="name">The name of the parameter</param>
	/// <param name="description">The description of the parameter</param>
	/// <param name="type">The type of the parameter</param>
	public Parameter(string name, string description, Type type) {
		this.name = name;
		this.description = description;
		this.type = type;
	}
}
