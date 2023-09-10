
namespace Manila.Scripting.API {
	/// <summary>
	/// The main Manila Build System API
	/// </summary>
	public static class Manila {
		/// <summary>
		/// Creates a new task
		/// </summary>
		/// <param name="name">the name of the task</param>
		/// <returns>the task object</returns>
		public static Task task(string name) {
			return new Task(name);
		}
	}
}
