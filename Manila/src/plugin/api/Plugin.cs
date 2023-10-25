
using System.Net;
using Manila.Core;
using Manila.Scripting;
using Manila.Utils;

namespace Manila.Plugin.API;

/// <summary>
/// Represents a base class for manila plugins.
/// </summary>
public abstract class Plugin {
	/// <summary>
	/// Gets the unique identifier of the plugin.
	/// </summary>
	public string id { get; }

	/// <summary>
	/// Gets the metadata associated with the plugin.
	/// </summary>
	public PluginManager.Meta meta { get; }

	internal readonly Dictionary<string, Storage> storages = new Dictionary<string, Storage>();

	/// <summary>
	/// Initializes a new instance of the <see cref="Plugin"/> class.
	/// </summary>
	/// <param name="id">The ID of the plugin.</param>
	public Plugin(string id) {
		this.id = id;
		var meta = PluginManager.getMeta(id);
		this.meta = meta;
	}

	/// <summary>
	/// Logs debug information.
	/// </summary>
	/// <param name="data">The dynamic data to log.</param>
	public void debug(params dynamic[] data) {
		Logger.pluginDebug(id, data);
	}

	/// <summary>
	/// Logs debug information with an optional prefix.
	/// </summary>
	/// <param name="prefix">Determines whether to add a prefix.</param>
	/// <param name="data">The dynamic data to log.</param>
	public void debug(bool prefix, params dynamic[] data) {
		if (prefix) Logger.pluginDebug(id, data);
		else Logger.debug(data);
	}

	/// <summary>
	/// Logs informational messages.
	/// </summary>
	/// <param name="data">The dynamic data to log.</param>
	public void info(params dynamic[] data) {
		Logger.pluginInfo(id, data);
	}

	/// <summary>
	/// Logs informational messages with an optional prefix.
	/// </summary>
	/// <param name="prefix">Determines whether to add a prefix.</param>
	/// <param name="data">The dynamic data to log.</param>
	public void info(bool prefix, params dynamic[] data) {
		if (prefix) Logger.pluginInfo(id, data);
		else Logger.info(data);
	}

	/// <summary>
	/// Logs debug information with markup.
	/// </summary>
	/// <param name="data">The dynamic data to log.</param>
	public void markupDebug(params dynamic[] data) {
		Logger.pluginDebug(id, data);
	}

	/// <summary>
	/// Logs debug information with markup and an optional prefix.
	/// </summary>
	/// <param name="prefix">Determines whether to add a prefix.</param>
	/// <param name="data">The dynamic data to log.</param>
	public void markupDebug(bool prefix, params dynamic[] data) {
		if (prefix) Logger.pluginMarkupDebug(id, data);
		else Logger.debugMarkup(data);
	}

	/// <summary>
	/// Logs informational messages with markup.
	/// </summary>
	/// <param name="data">The dynamic data to log.</param>
	public void markupInfo(params dynamic[] data) {
		Logger.pluginMarkupInfo(id, data);
	}

	/// <summary>
	/// Logs informational messages with markup and an optional prefix.
	/// </summary>
	/// <param name="prefix">Determines whether to add a prefix.</param>
	/// <param name="data">The dynamic data to log.</param>
	public void markupInfo(bool prefix, params dynamic[] data) {
		if (prefix) Logger.pluginMarkupInfo(id, data);
		else Logger.infoMarkup(data);
	}

	/// <summary>
	/// Adds an object to the scripting engine.
	/// </summary>
	/// <param name="name">The name of the object.</param>
	/// <param name="obj">The object to add.</param>
	public void addObject(string name, object obj) {
		ScriptManager.addObject(name, obj);
	}

	/// <summary>
	/// Adds a type to the scripting engine.
	/// </summary>
	/// <param name="type">The type to add.</param>
	public void addType(Type type) {
		ScriptManager.addType(type);
	}

	/// <summary>
	/// Sets the default build config
	/// </summary>
	/// <param name="config">The build config</param>
	/// <exception cref="Exception">Gets thrown when a build config already has been set</exception>
	public void setBuildConfig(BuildConfig config) {
		if (ScriptManager.buildConfig != null) throw new Exception("Build config has already been set!");
		ScriptManager.buildConfig = config;
	}

	/// <summary>
	/// Initializes the plugin.
	/// </summary>
	public virtual void init() {
		foreach (var e in storages) {
			var file = FileUtils.getStorage(e.Value, this);
			if (!file.exists()) file.write("{}");
			e.Value.deserialize(file);
		}
	}

	/// <summary>
	/// Shuts down the plugin.
	/// </summary>
	public virtual void shutdown() {
		foreach (var e in storages) { e.Value.serialize(FileUtils.getStorage(e.Value, this)); }
	}

	/// <summary>
	/// Defines commands for the plugin within a CLI application.
	/// </summary>
	/// <param name="app">The CLI application to add commands to.</param>
	public virtual void commands(CLI.App app) { }
}
