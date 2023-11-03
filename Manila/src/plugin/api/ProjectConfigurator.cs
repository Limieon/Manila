
namespace Manila.Plugin.API;

/// <summary>
/// Abstract definition for project configuration classes
/// </summary>
public abstract class ProjectConfigurator {
	/// <summary>
	/// Initializes configuration fields to their default values
	/// </summary>
	public abstract void init();
	/// <summary>
	/// Checks for values in configuration fields
	/// </summary>
	public abstract void check();
	/// <summary>
	/// Returns a dictionary containing the field names and their values
	/// </summary>
	/// <returns></returns>
	public abstract Dictionary<string, dynamic> getProperties();
}
