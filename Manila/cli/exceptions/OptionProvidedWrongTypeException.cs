
namespace Manila.CLI.Exceptions {
	public class OptionProvidedWrongTypeException : Exception {
		public Option option { get; private set; }
		public string provided { get; private set; }
		public Option.Type typeProivded { get; private set; }

		public OptionProvidedWrongTypeException(Option o, Option.Type typeProivded, string provided) : base("Option '" + o.name + "' provided with type '" + typeProivded.ToString().ToLower() + "', but required type '" + o.type.ToString().ToLower() + "'!") {
			this.option = o;
			this.typeProivded = typeProivded;
			this.provided = provided;
		}
	}
}
