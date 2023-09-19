
using Manila.Scripting.API;
using Manila.Utils;

namespace Manila.Plugin.API;

/// <summary>
/// Represents a storage base class for plugins
/// Storage stay persistant between executions
/// </summary>
public abstract class Storage {
	/// <summary>
	/// Gets the unique identifier for this storage
	/// </summary>
	public readonly string id;

	/// <summary>
	/// Initializes a new instance of the <see cref="Storage"/> class with the specified identifier
	/// </summary>
	/// <param name="id">The unique identifier for the storage</param>
	public Storage(string id) {
		this.id = id;
	}

	/// <summary>
	/// Deserializes the storage from the specified ManilaFile
	/// </summary>
	/// <param name="file">The ManilaFile from which to deserialize the storage</param>
	public abstract void deserialize(ManilaFile file);

	/// <summary>
	/// Serializes the storage to the specified ManilaFile
	/// </summary>
	/// <param name="file">The ManilaFile to which the storage should be serialized</param>
	public abstract void serialize(ManilaFile file);

	/// <summary>
	/// Saves the storage using the provided plugin.
	/// </summary>
	/// <param name="plugin">The Plugin instance used to save the storage</param>
	public void save(Plugin plugin) {
		serialize(FileUtils.getStorage(this, plugin));
	}

	/// <summary>
	/// Loads the storage using the provided plugin.
	/// If the storage file does not exist, it will be initialized with an empty JSON object
	/// </summary>
	/// <param name="plugin">The Plugin instance used to load the storage</param>
	public void load(Plugin plugin) {
		var file = FileUtils.getStorage(this, plugin);
		if (!file.exists()) file.write("{}");
		deserialize(file);
	}
}
