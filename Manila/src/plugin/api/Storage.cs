
using Manila.Scripting.API;
using Manila.Utils;

namespace Manila.Plugin.API;

public abstract class Storage {
	public readonly string id;

	public Storage(string id) {
		this.id = id;
	}

	public abstract void deserialize(ManilaFile file);
	public abstract void serialize(ManilaFile file);

	public void save(Plugin plugin) {
		serialize(FileUtils.getStorage(this, plugin));
	}
	public void load(Plugin plugin) {
		var file = FileUtils.getStorage(this, plugin);
		if (!file.exists()) file.write("{}");
		deserialize(file);
	}
}
