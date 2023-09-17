
using Manila.Commands;
using Manila.Plugin;
using Manila.Scripting;
using Manila.Utils;
using Manila.CLI.Exceptions;
using Manila.Core;
using Spectre.Console;
using Manila.Scripting.API;
using Manila.Data;

namespace Manila;

class Launcher {
	public static void Main(string[] args) {
#if DEBUG
		Directory.SetCurrentDirectory("../run/");
#endif
		AnsiConsole.Write(new FigletText(FigletFont.Parse(CLI.Fonts.Doom.get()), "Manila").LeftJustified().Color(Color.DodgerBlue3));

		var verbose = false;
		foreach (var s in args) {
			if (s == "-v" || s == "--verbose") verbose = true;
		}

		Logger.init(verbose);

		var rootDir = Scripting.API.Manila.dir(Directory.GetCurrentDirectory());
		var app = new CLI.App("Manila", "A Build System written in [green4]C#[/] using [yellow]JavaScript[/] as Build Scripts");
		FileUtils.init(rootDir);

		var workspace = new Data.Workspace();

		Logger.debug("Root Dir:", rootDir.getPath());
		if (rootDir.join(".manila").exists() && workspace.data.name != null) {
			PluginManager.init();
			PluginManager.loadPlugins(app);

			ScriptManager.init(workspace);
			ScriptManager.runWorkspaceFile();
			if (args.Length > 0 && args[0].StartsWith(":")) {
				ScriptManager.getTask(args[0]).execute();
			}
		}

		try {
			app.setDefaultCommand(new CommandManila())
				.setHelpCommand(new Commands.CommandHelp())
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
		} finally {
			PluginManager.shutdown();
			ScriptManager.shutdown();
		}
	}
}
