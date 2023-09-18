
using System.Runtime.InteropServices;

namespace Manila.Core;

/// <summary>
/// Represents the base of a build configuration
/// </summary>
public class BuildConfig {
	/// <summary>
	/// Gets the name of the platform (e.g., "windows", "linux", "osx", "freebsd", or "unknown").
	/// </summary>
	public readonly string platform;

	/// <summary>
	/// Initializes a new instance of the <see cref="BuildConfig"/> class.
	/// </summary>
	/// <remarks>
	/// This constructor parses command-line arguments prefixed with "--c:" and assigns values to corresponding fields in this class.
	/// Supported field types include string, int, float, double, and enum types.
	/// </remarks>
	public BuildConfig() {
		platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows" :
			RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
			RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" :
			RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) ? "freebsd" : "unknown";

		var args = Environment.GetCommandLineArgs();
		for (var i = 0; i < args.Length; ++i) {
			var s = args[i];
			if (s.StartsWith("--c:")) {
				var key = s[4..];
				dynamic val = args.Length > i + 1 ? args[i + 1] : true;

				var field = GetType().GetField(key);
				if (field == null) throw new FieldAccessException($"Field '{key}' was not found on '{GetType().FullName}'!");
				if (field.FieldType.IsEnum) {
					var func = GetType().GetMethod(field.FieldType.Name.ToLower() + "FromString");
					field.SetValue(this, func.Invoke(null, new object[] { val }));
				} else if (field.FieldType == typeof(int)) {
					field.SetValue(this, int.Parse(val));
				} else if (field.FieldType == typeof(float)) {
					field.SetValue(this, float.Parse(val));
				} else if (field.FieldType == typeof(double)) {
					field.SetValue(this, double.Parse(val));
				} else if (field.FieldType == typeof(string)) {
					var validField = GetType().GetField($"{field.Name}_VALID".ToUpper());
					if (validField != null) {
						var valid = (List<string>) validField.GetValue(null);
						foreach (var v in valid) { System.Console.WriteLine(v); }

						if (!valid.Contains(val)) throw new Exception($"Config '{field.Name}' has an invliad value!");
					}
					field.SetValue(this, val);
				}
			}
		}
	}
}
