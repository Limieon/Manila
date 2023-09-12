
using System.Reflection;
using Manila.Scripting.API;

namespace Manila.Plugin;

public static class PluginManager {
	public static readonly ManilaDirectory PLUGIN_ROOT = new ManilaDirectory(".manila/plugins");

	public static List<API.Plugin> plugins { get; private set; }

	public static void loadPlugins() {
		plugins = PLUGIN_ROOT.files().SelectMany(f => {
			Assembly pluginAssembly = loadPlugin(f.getPath());
			return createPlugins(pluginAssembly);
		}).ToList();
	}

	public static void init() {
		foreach (var p in plugins) {
			p.init();
		}
	}

	private static Assembly loadPlugin(string relPath) {
		string root = Path.GetFullPath(relPath);
		Console.WriteLine(root);

		string pluginLocation = root;
		Console.WriteLine($"Loading plugins from: {pluginLocation}");
		PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
		return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
	}
	private static IEnumerable<API.Plugin> createPlugins(Assembly assembly) {
		int count = 0;

		foreach (Type type in assembly.GetTypes()) {
			if (typeof(API.Plugin).IsAssignableFrom(type)) {
				API.Plugin res = Activator.CreateInstance(type) as API.Plugin;
				if (res != null) {
					count++;
					yield return res;
				}
			}
		}

		if (count == 0) {
			string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
			throw new ApplicationException(
				$"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
				$"Available types: {availableTypes}");
		}
	}

	public static void shutdown() {
	}
}
