
using Microsoft.ClearScript;

namespace Manila.Utils;

public static class ScriptUtils {
	public static T[] toArray<T>(ScriptObject obj) {
		if (obj["length"] == null || obj["length"].GetType() == typeof(Undefined)) throw new ArgumentException("Cannot convert ScriptObject array; ScriptObject is missing a length property!");

		List<T> l = new List<T>();

		for (var i = 0; i < (int) obj["length"]; ++i) {
			l.Add((T) obj[i]);
		}
		return l.ToArray();
	}

	public static Dictionary<string, T> toMap<T>(ScriptObject obj) {
		Dictionary<string, T> o = new Dictionary<string, T>();
		foreach (var p in obj.PropertyNames) {
			o.Add(p, (T) obj[p]);
		}

		return o;
	}

	public static string getTaskName(Scripting.API.Task task) {
		if (task.project == null) { return task.name; }
		return task.project.id + task.name;
	}
}
