
namespace Manila.CLI.Exceptions {
	class ParameterNotProivdedException : Exception {
		public Parameter parameter { get; private set; }
		public Command command { get; private set; }

		public ParameterNotProivdedException(Parameter p, Command c) {
			this.parameter = p;
			this.command = c;
		}
	}
}
