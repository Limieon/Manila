
using System.Runtime.InteropServices;

namespace Manila.CLI {
	public class App {
		private string name { get; }
		private string description { get; }
		public Command? defaultCommand { get; private set; }
		private List<Command> commands;

		public App(string name, string description) {
			this.name = name;
			this.description = description;
			this.commands = new List<Command>();
		}

		public App setDefaultCommand(Command command) {
			this.defaultCommand = command;
			return this;
		}
		public App addCommand(Command command) {
			this.commands.Add(command);
			return this;
		}

		public void parse(string[] args) {
			List<object> parameters = new List<object>();
			Dictionary<string, object> options = new Dictionary<string, object>();

			{
				int i = 0;
				while (i < args.Length) {
					string arg = args[i];

					if (arg.StartsWith("--")) {
						string? nextArg = args.Length > i + 1 ? args[i + 1] : null;
						if (nextArg != null) options.Add(arg.Substring(2), nextArg);
						else options.Add(arg.Substring(2), true);

						i += 2;
						continue;
					}

					parameters.Add(arg);
					i++;
				}
			}

			if (parameters.Count < 1) {
				this.defaultCommand?.onExecute(new Dictionary<string, object>(), options);
				return;
			}

			string cmd = args[0];
			foreach (Command c in commands) {
				if (c.name == cmd) {
					Dictionary<string, object> parsedParams = new Dictionary<string, object>();

					if ((parameters.Count - 1) < c.parameters.Count) {
						throw new Exceptions.ParameterNotProivdedException(c.parameters[parameters.Count - 1], c);
					}

					for (int i = 0; i < c.parameters.Count; ++i) {
						parsedParams.Add(c.parameters[i].name, parameters[i + 1]);
					}

					c.onExecute(parsedParams, options);
				}
			}
		}
	}
}
