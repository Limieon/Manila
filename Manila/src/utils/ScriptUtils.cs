
using Manila.Core;
using Manila.Plugin;
using Manila.Scripting.Exceptions;
using Microsoft.ClearScript;

namespace Manila.Utils;

/// <summary>
/// Provides utlity functions to interact with scripts
/// </summary>
public static class ScriptUtils {
	/// <summary>
	/// Converts a ScriptObject into an array of a specified type
	/// </summary>
	/// <typeparam name="T">The type of elements in the array</typeparam>
	/// <param name="obj">The ScriptObject to convert</param>
	/// <returns>An array containing the elements of the ScriptObject</returns>
	/// <exception cref="ArgumentException">Thrown if the ScriptObject is missing a length property</exception>
	public static T[] toArray<T>(ScriptObject obj) {
		if (obj["length"] == null || obj["length"].GetType() == typeof(Undefined)) throw new ArgumentException("Cannot convert ScriptObject array; ScriptObject is missing a length property!");

		List<T> l = new List<T>();

		for (var i = 0; i < (int) obj["length"]; ++i) {
			l.Add((T) obj[i]);
		}
		return l.ToArray();
	}

	/// <summary>
	/// Converts a ScriptObject into a dictionary of strings and values of a specified type
	/// </summary>
	/// <typeparam name="T">The type of values in the dictionary</typeparam>
	/// <param name="obj">The ScriptObject to convert </param>
	/// <returns>A dictionary containing the properties of the ScriptObject</returns>
	public static Dictionary<string, T> toMap<T>(ScriptObject obj) {
		Dictionary<string, T> o = new Dictionary<string, T>();
		foreach (var p in obj.PropertyNames) {
			o.Add(p, (T) obj[p]);
		}

		return o;
	}

	/// <summary>
	/// Gets the name of a task from a Scripting.API.Task object
	/// </summary>
	/// <param name="task">The Scripting.API.Task object</param>
	/// <returns>The name of the task, including the project ID if it exists</returns>
	public static string getTaskName(Scripting.API.Task task) {
		if (task.project == null) { return task.name; }
		return task.project.id + task.name;
	}

	public static async Task<int> executeTask(Manila.Scripting.API.Task t) {
		try {
			long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			var res = await ScriptManager.executeTask(t);
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

			Logger.debug("Avilable Tasks:");
			foreach (var ta in ScriptManager.getTasks()) {
				Logger.debug(ta.name);
			}

			return -1;
		}
	}
}
