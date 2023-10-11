
namespace Manila.Scripting.API;

public class ENV {
	private Dictionary<string, object> data;

	public ENV() {
		data = new Dictionary<string, object>();
		load();
	}

	private void load() {
		var envFile = new ManilaFile(".env");
		if (!envFile.exists()) return;

		var lines = envFile.read().Split("\n");
		foreach (var l in lines) {
			var parts = l.Trim().Split("=");
			if (!data.ContainsKey(parts[0]))
				if (parts.Length == 2)
					data.Add(parts[0], parse(parts[1]));
		}
	}
	private object parse(string value) {
		if (int.TryParse(value, out int iv)) return iv;
		else if (double.TryParse(value, out double dv)) return dv;
		else if (bool.TryParse(value, out bool bv)) return bv;
		return value;
	}

	public object? get(string key, object vDefault) {
		return has(key) ?
			get(key) :
			Environment.GetEnvironmentVariable(key) == null ?
				vDefault :
				Environment.GetEnvironmentVariable(key);
	}
	public object get(string key) { return data[key]; }
	public bool has(string key) { return Environment.GetEnvironmentVariable(key) != null || data.ContainsKey(key); }
}
