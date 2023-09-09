
namespace Manila.CLI.Exceptions {
	public class ParameterProvidedWrongTypeException : Exception {
		public Parameter parameter { get; private set; }
		public string provided { get; private set; }

		public ParameterProvidedWrongTypeException(Parameter p, string provided) : base("Parameter '" + p.name + "' provided with type string, but required type '" + p.type.ToString().ToLower() + "'!") {
			this.parameter = p;
			this.provided = provided;
		}
	}
}
