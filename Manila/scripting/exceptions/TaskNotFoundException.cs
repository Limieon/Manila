
namespace Manila.Scripting.Exceptions {
	/// <summary>
	/// Gets thrown when a task could not be found
	/// </summary>
	public class TaskNotFoundException : Exception {
		/// <summary>
		/// The task's name that was not found
		/// </summary>
		public string name { get; private set; }
		/// <summary>
		/// The taks that searched for it
		/// </summary>
		public API.Task? task { get; private set; }

		/// <summary>
		/// Creates a new TaskNotFoundException when a task was not found
		/// </summary>
		/// <param name="name">the taks name that wasn't found</param>
		public TaskNotFoundException(string name) : base("Task '" + name + "' could not be found!") {
			this.name = name;
		}
		/// <summary>
		/// Creates a new TaskNotFoundException when the task as a dependency was not found
		/// </summary>
		/// <param name="name">the taks name that wasn't found</param>
		/// <param name="task">the task that searched for it</param>
		public TaskNotFoundException(string name, API.Task task) : base("Task '" + name + "' could not be found! (Required by '" + task.name + "')") {
			this.name = name;
			this.task = task;
		}
	}
}
