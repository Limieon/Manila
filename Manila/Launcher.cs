
using Manila.Commands;

namespace Manila {
	public class Launcher {
		public static void Main(String[] args) {
			try {
				new CLI.App("manila", "manila buildsystem")
					.setDefaultCommand(new CommandManila())
					.addCommand(new CommandInit())
					.parse(args);
			} catch (CLI.Exceptions.ParameterNotProivdedException e) {
				Console.WriteLine("Missing Parameter '" + e.parameter.name + "' on Command '" + e.command.name + "'!");
			} catch (CLI.Exceptions.ParameterProvidedWrongTypeException e) {
				Console.WriteLine("Paremeter '" + e.parameter.name + "' has wrong type (" + e.parameter.type.ToString().ToLower() + " required)!");
			}
		}
	}
}
