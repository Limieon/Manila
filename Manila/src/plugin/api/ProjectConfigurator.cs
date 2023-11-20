
using Manila.Core;

namespace Manila.Plugin.API;

/// <summary>
/// Abstract definition for project configuration classes, implemented by plugins
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

	/// <summary>
	/// Gets called when the workspace is needed to be generated
	/// </summary>
	/// <param name="toolset"></param>
	public abstract void generate(Workspace workspace, string toolset);
	/// <summary>
	/// Gets called when the workspace is needed to be build
	/// </summary>
	/// <param name="toolset"></param>
	public abstract void build(Workspace workspace, string toolset);
}
