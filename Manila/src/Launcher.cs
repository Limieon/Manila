
using Manila.Commands;
using Manila.Scripting;

namespace Manila {
	class Launcher {
		public static void Main(string[] args) {
#if DEBUG
			Directory.SetCurrentDirectory("../run/");
#endif

			if (args.Length > 0 && args[0].StartsWith(":")) ScriptEngine.getInstance().run().getTask(args[0][1..]).execute();

			try {
				new CLI.App("Manila", "A Build System written in [green4]C#[/] using [yellow]JavaScript[/] as Build Scripts")
					.setDefaultCommand(new CommandManila())
					.setHelpCommand(new CommandHelp())
					.addCommand(new CommandInit())
					.parse(args);
			} catch (CLI.Exceptions.ParameterNotProivdedException e) {
				Console.WriteLine("Missing Parameter '" + e.parameter.name + "' on Command '" + e.command.name + "'!");
			} catch (CLI.Exceptions.ParameterProvidedWrongTypeException e) {
				Console.WriteLine("Paremeter '" + e.parameter.name + "' has wrong type (" + e.parameter.type.ToString().ToLower() + " required)!");
			} catch (CLI.Exceptions.OptionProvidedNotFoundExceptions e) {
				Console.WriteLine("Could not find option '" + e.option + "' on command '" + e.command.name + "'!");
			}
		}
	}
}
