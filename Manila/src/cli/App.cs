
using Manila.CLI.Exceptions;

namespace Manila.CLI {
	public class App {
		public string name { get; private set; }
		public string description { get; private set; }
		public Command? defaultCommand { get; private set; }
		public CommandHelp? helpCommand { get; private set; }
		public List<Command> commands { get; private set; }

		public App(string name, string description) {
			this.name = name;
			this.description = description;
			this.commands = new List<Command>();
		}

		public App setDefaultCommand(Command command) {
			this.defaultCommand = command;
			return this;
		}
		public App setHelpCommand(CommandHelp command) {
			this.helpCommand = command;
			return this;
		}
		public App addCommand(Command command) {
			this.commands.Add(command);
			return this;
		}

		public void runHelpCommand() {
			helpCommand?.printHelp(this);
		}

		public void parse(string[] args) {
			List<string> parameters = new List<string>();
			Dictionary<string, object> options = new Dictionary<string, object>();

			{
				int i = 0;
				while (i < args.Length) {
					string arg = args[i];

					if (arg.StartsWith("--")) {
						string? nextArg = args.Length > i + 1 ? args[i + 1] : null;
						if (nextArg != null && !nextArg.StartsWith("-")) {
							options.Add(arg.Substring(2), nextArg);
							i += 2;
						} else {
							options.Add(arg[2..], true);
							i++;
						}

						continue;
					}

					parameters.Add(arg);
					i++;
				}
			}

			if (parameters.Count < 1) {
				defaultCommand?.onExecute(new Dictionary<string, object>(), options);
				defaultCommand?.onExecute(new Dictionary<string, object>(), options, this);
				return;
			}

			string cmd = args[0].ToLower();
			if (cmd == "help") {
				this.helpCommand?.printHelp(this);
				return;
			}

			foreach (Command c in commands) {
				if (c.name == cmd) {
					Dictionary<string, object> parsedParams = new Dictionary<string, object>();
					Dictionary<string, object> parsedOpts = new Dictionary<string, object>();

					foreach (KeyValuePair<string, Option> entry in c.options) {
						if (!parsedOpts.ContainsKey(entry.Key)) parsedOpts.Add(entry.Key, entry.Value.vDefault);
					}

					foreach (KeyValuePair<string, object> entry in options) {
						if (c.options.ContainsKey(entry.Key)) {
							parsedOpts[entry.Key] = c.options[entry.Key].parse(entry.Value);
						} else {
							parsedOpts.Add(entry.Key, entry.Value);
						}
					}

					if (parsedOpts.ContainsKey("help")) {
						this.helpCommand?.printHelp(c);
						return;
					}

					if ((parameters.Count - 1) < c.parameters.Count) {
						throw new ParameterNotProivdedException(c.parameters[parameters.Count - 1], c);
					}

					for (int i = 0; i < c.parameters.Count; ++i) {
						parsedParams.Add(c.parameters[i].name, c.parameters[i].parse(parameters[i + 1]));
					}

					c.onExecute(parsedParams, parsedOpts);
					c.onExecute(parsedParams, parsedOpts, this);
				}
			}
		}
	}
}
