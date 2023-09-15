
using System.Reflection;
using Manila.Scripting.API;
using Manila.Scripting.Exceptions;
using Manila.Utils;

namespace Manila.Plugin;

public static class PluginManager {
	public class Meta {
		public string name { get; set; }
		public string description { get; set; }
		public List<string> authors { get; set; }
		public string version { get; set; }
	}

	public static readonly ManilaDirectory PLUGIN_ROOT = new ManilaDirectory(".manila/plugins");

	public static List<API.Plugin> plugins { get; private set; }
	private static Dictionary<string, Meta> pluginMetas;

	public static void loadPlugins(CLI.App app) {
		plugins = PLUGIN_ROOT.files().SelectMany(f => {
			Assembly pluginAssembly = loadPlugin(f.getPath());
			return createPlugins(pluginAssembly);
		}).ToList();

		foreach (var p in plugins) {
			p.init();
			p.commands(app);
		}
	}

	public static void init() {
		if (!FileUtils.pluginsFile.exists()) { FileUtils.pluginsFile.write("{}"); }
		pluginMetas = FileUtils.pluginsFile.deserializeJSON<Dictionary<string, Meta>>();
		FileUtils.pluginsFile.serializeJSON(pluginMetas, true);
	}

	/// <summary>
	/// Gets the metadata for a plugin
	/// </summary>
	/// <param name="id">the plugin id</param>
	/// <exception cref="PluginNotFoundException"></exception>
	public static Meta getMeta(string id) {
		if (!pluginMetas.ContainsKey(id)) throw new PluginNotFoundException(id);
		return pluginMetas[id];
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
		foreach (var p in plugins) {
			p.shutdown();
		}
	}
}
