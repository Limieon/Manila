
using Manila.CLI.Exceptions;

namespace Manila.CLI;

/// <summary>
/// Represents a command-line option with various configuration settings
/// </summary>
public class Option {
	/// <summary>
	/// Defines the possible types of command-line options
	/// </summary>
	public enum Type {
		/// <summary>
		/// Represents a string option
		/// </summary>
		STRING,
		/// <summary>
		/// Represents a numeric option
		/// </summary>
		NUMBER,
		/// <summary>
		/// Represents a boolean flag option
		/// </summary>
		FLAG
	}

	/// <summary>
	/// Gets the type of the option
	/// </summary>
	public Type type { get; private set; }

	/// <summary>
	/// Gets the name of the option
	/// </summary>
	public string name { get; private set; }

	/// <summary>
	/// Gets the description of the option
	/// </summary>
	public string description { get; private set; }

	/// <summary>
	/// Gets the alias of the option
	/// </summary>
	public string alias { get; private set; }

	/// <summary>
	/// Gets the default value of the option
	/// </summary>
	public object vDefault { get; private set; }

	/// <summary>
	/// Parses the specified value based on the option's type
	/// </summary>
	/// <param name="value">The value to parse</param>
	/// <returns>The parsed value</returns>
	public object parse(object value) {
		if (this.type == Type.STRING) return (string) value;
		if (this.type == Type.FLAG) {
			if (value.GetType() == typeof(bool)) return true;
			throw new OptionProvidedWrongTypeException(this, Type.STRING, (string) value);
		}
		if (this.type == Type.NUMBER) {
			try {
				return int.Parse((string) value);
			} catch (Exception) {
				throw new OptionProvidedWrongTypeException(this, value.GetType() == typeof(bool) ? Type.FLAG : Type.STRING, (string) value);
			}
		}

		// Won't be reached
		return null;
	}

	/// <summary>
	/// Initializes a new instance of the Option class with the specified settings
	/// </summary>
	/// <param name="name">The name of the option</param>
	/// <param name="description">The description of the option</param>
	/// <param name="alias">The alias of the option</param>
	/// <param name="vDefault">The default value of the option</param>
	/// <param name="type">The type of the option</param>
	public Option(string name, string description, string alias = "", object vDefault = null, Type type = Type.FLAG) {
		this.name = name;
		this.description = description;
		this.alias = alias;
		this.type = type;
		this.vDefault = vDefault == null ? false : vDefault;
	}

	/// <summary>
	/// Initializes a new instance of the Option class with the specified settings
	/// </summary>
	/// <param name="name">The name of the option</param>
	/// <param name="description">The description of the option</param>
	/// <param name="vDefault">The default value of the option</param>
	/// <param name="type">The type of the option</param>
	public Option(string name, string description, object vDefault, Type type) : this(name, description, "", vDefault, type) {
	}
}
