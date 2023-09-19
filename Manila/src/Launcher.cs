
using Manila.Commands;
using Manila.Plugin;
using Manila.Utils;
using Manila.CLI.Exceptions;
using Manila.Core;
using Spectre.Console;
using Manila.Scripting.Exceptions;
using Manila.Core.Exceptions;

namespace Manila;

class Launcher {
	public static async Task<int> Main(string[] args) {
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

		var workspace = new Data.Workspace();
		FileUtils.init(rootDir, workspace.data.name != null);

		Logger.debug("Root Dir:", rootDir.getPath());
		if (rootDir.join(".manila").exists() && workspace.data.name != null) {
			PluginManager.init();
			PluginManager.loadPlugins(app);

			if (args.Length > 0 && args[0].StartsWith(":")) {
				ScriptManager.init(workspace);

				try {
					ScriptManager.runWorkspaceFile();
				} catch (FileNotFoundException e) {
					Logger.infoMarkup($"[red]{e.Message}[/]");
					Logger.exception(e);

					return -1;
				} catch (PropertyNotFoundException e) {
					Logger.infoMarkup($"[red]Project[/] [blue]{e.project.id}[/] [red]does not contain property[/] [blue]{e.property}[/][red]![/]");
					Logger.exception(e);

					return -1;
				} catch (Exception e) {
					Logger.infoMarkup("[red]Could not find[/] [yellow]Manila.js[/] [red]build script![/]");
					Logger.infoMarkup("Directory does not seem to be a [blue]script environment[/]!");
					Logger.exception(e);

					return -1;
				}

				try {
					long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
					var res = await ScriptManager.executeTask(ScriptManager.getTask(args[0]));
					long duration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - start;

					if (res) {
						Logger.info();
						Logger.infoMarkup($"[green]Task Successful![/] [gray]Took[/] [cyan]{FormattingUtils.stringifyDuration(duration)}[/]");
						return 0;
					} else {
						Logger.info();
						Logger.infoMarkup($"[red]Task Successful![/] [gray]Took[/] [cyan]{FormattingUtils.stringifyDuration(duration)}[/]");
						return -1;
					}
				} catch (TaskNotFoundException e) {
					Logger.infoMarkup($"[red]Task[/] [yellow]{e.name}[/] [red]could not be found![/]");
					Logger.exception(e);

					return -1;
				}
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

		return 0;
	}
}
