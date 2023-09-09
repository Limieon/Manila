
namespace Manila.CLI {
	public class Parameter {
		public enum Type {
			STRING,
			NUMBER
		}

		public Type type { get; private set; }
		public string name { get; private set; }
		public string description { get; private set; }

		public Parameter(string name, string description, Type type) {
			this.name = name;
			this.description = description;
			this.type = type;
		}
	}
}
