
using Manila.Commands;
using Manila.Plugin;
using Manila.Utils;
using Manila.CLI.Exceptions;
using Manila.Core;
using Manila.Core.Exceptions;
using Spectre.Console;

namespace Manila;

class Launcher {
	public static async Task<int> Main(string[] args) {
#if DEBUG
		if (Directory.Exists("../run")) Directory.SetCurrentDirectory("../run");
		else Directory.SetCurrentDirectory("../../../../run/");
#endif
		AnsiConsole.Write(new FigletText(FigletFont.Parse(CLI.Fonts.Doom.get()), "Manila").LeftJustified().Color(Color.DodgerBlue3));

		var verbose = false;
		foreach (var s in args) {
			if (s == "--verbose") verbose = true;
		}

		Logger.init(verbose);

		var rootDir = Scripting.API.Manila.dir(Directory.GetCurrentDirectory());
		var app = new CLI.App("Manila", "A extensible Build System written in [green4]C#[/] using [yellow]JavaScript[/] as Build Scripts", "1.0.0");

		var workspace = new Data.Workspace();
		FileUtils.init(null, workspace.data.name != null);

		ScriptManager.init(workspace);
		PluginManager.init();

		try {
			PluginManager.loadPlugins(app);
		} catch (ConfigNotFoundException e) {
			Logger.infoMarkup($"[red]Config called[/] [yellow]{e.name}[/] [red]was not found![/]");
			Logger.exception(e);

			return -1;
		}

		try {
			app.setDefaultCommand(new CommandManila())
				.setHelpCommand(new CommandHelp())
				.addCommand(new CommandInit())
				.addCommand(new CommandBuild())
				.addCommand(new CommandRebuild())
				.addCommand(new CommandRun())
				.addCommand(new CommandTasks())
				.addCommand(new CommandPlugins())
				.addCommand(new CommandProjects())
				.addCommand(new CommandGenerate())
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
		} catch (Exception e) {
			Logger.infoMarkup($"[red]An unknown exception occured![/] [yellow]{e.Message}[/]");
			Logger.exception(e);
		}

		return 0;
	}
}
