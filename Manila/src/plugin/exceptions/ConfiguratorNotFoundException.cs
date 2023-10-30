
namespace Manila.Scripting.Exceptions {
	/// <summary>
	/// Gets thrown when a task could not be found
	/// </summary>
	public class ConfiguratorNotFoundException : Exception {
		/// <summary>
		/// The plugin that should contained it
		/// </summary>
		public Plugin.API.Plugin plugin { get; private set; }

		/// <summary>
		/// The configurator that id was not found
		/// </summary>
		public string id { get; private set; }

		/// <summary>
		/// Creates a new TaskNotFoundException when a task was not found
		/// </summary>
		/// <param name="id">The plugin id that was not found</param>
		public ConfiguratorNotFoundException(Plugin.API.Plugin plugin, string id) : base($"The configurator that should be contained within '{plugin.id}' was not found! (Searched: '{id}')") {
			this.plugin = plugin;
			this.id = id;
		}
	}
}
