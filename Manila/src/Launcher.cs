
using Manila.Commands;
using Manila.Plugin;
using Manila.Scripting;
using Manila.Utils;
using Manila.CLI.Exceptions;

namespace Manila;

class Launcher {
	public static void Main(string[] args) {
#if DEBUG
		Directory.SetCurrentDirectory("../run/");
#endif

		var verbose = false;
		foreach (var s in args) {
			if (s == "-v" || s == "--verbose") verbose = true;
		}

		Logger.init(verbose);

		var rootDir = Scripting.API.Manila.dir(Directory.GetCurrentDirectory());
		Logger.debug("Root Dir:", rootDir.getPath());
		FileUtils.init(rootDir);
		PluginManager.loadPlugins();
		PluginManager.init();
		if (args.Length > 0 && args[0].StartsWith(":")) {
			ScriptEngine.getInstance().run().getTask(args[0][1..]).execute();
		}

		try {
			new CLI.App("Manila", "A Build System written in [green4]C#[/] using [yellow]JavaScript[/] as Build Scripts")
				.setDefaultCommand(new CommandManila())
				.setHelpCommand(new CommandHelp())
				.addCommand(new CommandInit())
				.parse(args);
		} catch (ParameterNotProivdedException e) {
			Console.WriteLine("Missing Parameter '" + e.parameter.name + "' on Command '" + e.command.name + "'!");
			Logger.exception(e);
		} catch (ParameterProvidedWrongTypeException e) {
			Console.WriteLine("Paremeter '" + e.parameter.name + "' has wrong type (" + e.parameter.type.ToString().ToLower() + " required)!");
			Logger.exception(e);
		} catch (OptionProvidedNotFoundExceptions e) {
			Console.WriteLine("Could not find option '" + e.option + "' on command '" + e.command.name + "'!");
			Logger.exception(e);
		}
	}
}
