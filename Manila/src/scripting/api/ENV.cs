
using System.Data.Common;
using System.Text.Json.Nodes;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Manila.Scripting.API;

/// <summary>
/// API for enviornment variables
/// </summary>
public class ENV {
	private PropertyBag data;

	internal ENV() {
		data = new PropertyBag();
		load();
	}

	private void load() {
		var envFile = new ManilaFile(".env");

		foreach (string key in Environment.GetEnvironmentVariables().Keys) {
			data.Add(key, parse(Environment.GetEnvironmentVariable(key)));
		}

		if (!envFile.exists()) return;

		var lines = envFile.read().Split("\n");
		foreach (var l in lines) {
			var parts = l.Trim().Split("=");
			if (!data.ContainsKey(parts[0]))
				if (parts.Length == 2)
					data.Add(parts[0], parse(parts[1]));
		}
	}
	private object? parse(string? value) {
		if (value == null) return null;

		if (int.TryParse(value, out int iv)) return iv;
		else if (double.TryParse(value, out double dv)) return dv;
		else if (bool.TryParse(value, out bool bv)) return bv;
		return value;
	}

	/// <summary>
	/// Get a value by its key
	/// </summary>
	/// <param name="key">The key</param>
	/// <param name="vDefault">The default value when key is not found</param>
	/// <returns>The value or the default</returns>
	public object? get(string key, object? vDefault = null) {
		return has(key) ? data[key] : vDefault;
	}

	public object get() {
		return data;
	}

	/// <summary>
	/// Checks if a variable is present
	/// </summary>
	/// <param name="key">The key</param>
	/// <returns>true: exists</returns>
	public bool has(string key) { return data.ContainsKey(key); }
}
