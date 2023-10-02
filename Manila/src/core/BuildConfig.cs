
using System.Runtime.InteropServices;
using Manila.Core.Exceptions;

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
	/// This constructor parses command-line arguments prefixed with "--c:" and assigns values to corresponding fields in this class (or a derived clsass).
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
				dynamic val = true;

				if (args.Length > i + 1) {
					if (!args[i + 1].StartsWith("-")) val = args[i + 1];
				}

				var field = GetType().GetField(key);
				if (field == null) throw new ConfigNotFoundException(key, GetType());
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
						if (!valid.Contains(val)) throw new Exception($"Config '{field.Name}' has an invliad value!");
					}
					field.SetValue(this, val);
				} else if (field.FieldType == typeof(bool)) {
					field.SetValue(this, val);
				}
			}
		}
	}
}
