
using Manila.CLI.Exceptions;

namespace Manila.CLI {
	public class Option {
		public enum Type {
			STRING,
			NUMBER,
			FLAG
		}

		public Type type { get; private set; }
		public string name { get; private set; }
		public string description { get; private set; }
		public string alias { get; private set; }
		public object vDefault { get; private set; }

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

		public Option(string name, string description, object vDefault, string alias = "", Type type = Type.FLAG) {
			this.name = name;
			this.description = description;
			this.alias = alias;
			this.type = type;
			this.vDefault = vDefault;
		}
		public Option(string name, string description, object vDefault, Type type) : this(name, description, vDefault, "", type) {
		}
	}
}
