
using Manila.CLI.Exceptions;

namespace Manila.CLI;

public class Parameter {
	public enum Type {
		STRING,
		NUMBER
	}

	public Type type { get; private set; }
	public string name { get; private set; }
	public string description { get; private set; }

	public object parse(string proivded) {
		if (type == Type.STRING) return proivded;

		try {
			return int.Parse(proivded);
		} catch (Exception) {
			throw new ParameterProvidedWrongTypeException(this, proivded);
		}
	}

	public Parameter(string name, string description, Type type) {
		this.name = name;
		this.description = description;
		this.type = type;
	}
}
