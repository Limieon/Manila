
using System.Reflection;
using Manila.Plugin.API;
using Manila.Scripting.API;
using Manila.Scripting.Exceptions;
using Manila.Utils;

namespace Manila.Plugin;

/// <summary>
/// Manges and handles plugin loading
/// </summary>
public static class PluginManager {
	/// <summary>
	/// Metadata associated to plugins
	/// </summary>
	public class Meta {
		/// <summary>
		/// The name of the plugin (not id)
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// A brief description of the plugin
		/// </summary>
		public string description { get; set; }
		/// <summary>
		/// The authors that worked on the plugin
		/// </summary>
		public List<string> authors { get; set; }
		/// <summary>
		/// The installed plugin version
		/// </summary>
		public string version { get; set; }
	}

	/// <summary>
	/// The directory where plugin files are stored
	/// </summary>
	public static readonly ManilaDirectory PLUGIN_ROOT = new ManilaDirectory(".manila/plugins");

	/// <summary>
	/// The plugins objects that have been loaded
	/// </summary>
	public static List<API.Plugin> plugins { get; private set; }
	/// <summary>
	/// The metadata associated to plugins
	/// </summary>
	private static Dictionary<string, Meta> pluginMetas;

	private static bool initialized { get; set; }

	internal static void init() {
		if (!FileUtils.pluginsFile.exists()) { FileUtils.pluginsFile.write("{}"); }
		pluginMetas = FileUtils.pluginsFile.deserializeJSON<Dictionary<string, Meta>>();
		FileUtils.pluginsFile.serializeJSON(pluginMetas, true);

		initialized = true;
	}
	internal static void loadPlugins(CLI.App app) {
		plugins = PLUGIN_ROOT.files().SelectMany(f => {
			Assembly pluginAssembly = loadPlugin(f.getPath());
			return createPlugins(pluginAssembly);
		}).ToList();

		foreach (var p in plugins) {
			p.init();
			p.commands(app);
		}
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
		Logger.debug("Root:", root);

		string pluginLocation = root;
		Logger.debug($"Loading plugins from: {pluginLocation}");
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

	internal static void shutdown() {
		if (!initialized) return;
		foreach (var p in plugins) {
			p.shutdown();
		}
	}

	public static API.Plugin getPlugin(string id) {
		foreach (var p in plugins) { if (p.id == id) return p; }
		throw new PluginNotFoundException(id);
	}

	public static ProjectConfigurator getConfigurator(string id) {
		if (!id.Contains("/")) throw new ArgumentException("The project type ID must be in th following format: Plugin/type");
		var temp = id.Split("/");
		if (temp.Length < 2) throw new ArgumentException("The project type ID must be in th following format: Plugin/type");

		var plugin = getPlugin(temp[0]);
		ProjectConfigurator? conf = null;
		if (plugin.configurators.ContainsKey(temp[1])) conf = plugin.configurators[temp[1]];
		if (conf == null) throw new ConfiguratorNotFoundException(plugin, id);
		conf.init();
		return conf;
	}
}
