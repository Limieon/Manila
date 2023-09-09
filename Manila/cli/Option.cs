
namespace Manila.CLI {
	public class Option {
		public enum Type {
			STRING,
			NUMBER,
			FLAG
		}

		private Type type { get; }
		private string name { get; }
		private string description { get; }
		private string alias { get; }

		public Option(string name, string description, string alias = "", Type type = Type.FLAG) {
			this.name = name;
			this.description = description;
			this.alias = alias;
			this.type = type;
		}
		public Option(string name, string description, Type type) : this(name, description, "", type) {
		}
	}
}
