
using System.Runtime.InteropServices;

namespace Manila.Core;

public class BuildConfig {
	public readonly string platform;

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
				} else {
					field.SetValue(this, val);
				}
			}
		}
	}
}
