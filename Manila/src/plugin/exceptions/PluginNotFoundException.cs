
namespace Manila.Scripting.Exceptions {
	/// <summary>
	/// Gets thrown when a task could not be found
	/// </summary>
	public class PluginNotFoundException : Exception {
		/// <summary>
		/// The task's name that was not found
		/// </summary>
		public string id { get; private set; }

		/// <summary>
		/// Creates a new TaskNotFoundException when a task was not found
		/// </summary>
		/// <param name="name">the taks name that wasn't found</param>
		public PluginNotFoundException(string id) : base("Plugin Meta for plugin '" + id + "' could not be found!") {
			this.id = id;
		}
	}
}
