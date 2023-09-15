
using Manila.Scripting;
using Manila.Utils;

using Spectre.Console;

namespace Manila.Plugin.API;

public abstract class Plugin {
	public string id { get; }
	public PluginManager.Meta meta { get; }

	public Plugin(string id) {
		this.id = id;
		var meta = PluginManager.getMeta(id);
		this.meta = meta;
	}

	public void debug(params dynamic[] data) {
		Logger.pluginDebug(id, data);
	}
	public void debug(bool prefix, params dynamic[] data) {
		if(prefix) Logger.pluginDebug(id, data);
		else Logger.debug(data);
	}

	public void info(params dynamic[] data) {
		Logger.pluginInfo(id, data);
	}
	public void info(bool prefix, params dynamic[] data) {
		if(prefix) Logger.pluginInfo(id, data);
		else Logger.info(data);
	}

	public void markupDebug(params dynamic[] data) {
		Logger.pluginDebug(id, data);
	}
	public void markupDebug(bool prefix, params dynamic[] data) {
		if(prefix) Logger.pluginMarkupDebug(id, data);
		else Logger.debugMarkup(data);
	}

	public void markupInfo(params dynamic[] data) {
		Logger.pluginMarkupInfo(id, data);
	}
	public void markupInfo(bool prefix, params dynamic[] data) {
		if(prefix) Logger.pluginMarkupInfo(id, data);
		else Logger.infoMarkup(data);
	}

	public void addObject(string name, object obj) {
		ScriptEngine.getInstance().addObject(name, obj);
	}
	public void addType(Type type) {
		ScriptEngine.getInstance().addType(type);
	}

	public virtual void init() {}
	public virtual void shutdown() {}

	public virtual void commands(CLI.App app) {}
}
