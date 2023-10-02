
namespace Manila.Utils;

public static class CLIUtils {
	public static Dictionary<string, object> parseArguments(string[] args) {
		var res = new Dictionary<string, object>();

		var i = 0;
		while (i < args.Length) {
			var arg = args[i];

			if (arg.StartsWith("--")) {
				if (args.Contains("=")) {
					var temp = arg.Split("=");
					res.Add(temp[0][2..], parseValue(temp[1]));
					++i;
					continue;
				}

				if (args.Length > i + 1) {
					res.Add(arg[2..], parseValue(args[i + 1]));
					i += 2;
					continue;
				}

				res.Add(arg, true);

				i++;
				continue;
			}
			if (arg.StartsWith("-")) {
				foreach (var c in arg[1..]) {
					res.Add(c + "", true);
				}
			}

			++i;
		}

		return res;
	}

	public static object parseValue(string v) {
		{
			if (int.TryParse(v, out var value)) return value;
		}
		{
			if (float.TryParse(v, out var value)) return value;
		}
		{
			if (bool.TryParse(v, out var value)) return value;
		}
		return v;
	}
}
