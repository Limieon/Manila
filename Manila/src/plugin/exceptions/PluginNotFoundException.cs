
namespace Manila.Scripting.Exceptions {
	/// <summary>
	/// Gets thrown when a plugin could not be found
	/// </summary>
	public class PluginNotFoundException : Exception {
		/// <summary>
		/// The plugin id was not found
		/// </summary>
		public string id { get; private set; }

		/// <summary>
		/// Creates a new PluginNotFoundException when a plugin was not found
		/// </summary>
		/// <param name="id">The plugin id that was not found</param>
		public PluginNotFoundException(string id) : base("Plugin '" + id + "' could not be found!") {
			this.id = id;
		}
	}
}
