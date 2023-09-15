
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
}
