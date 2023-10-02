
using System.Text.RegularExpressions;
using Manila.CLI;
using Manila.Core;
using Manila.Utils;
using Microsoft.ClearScript;

namespace Manila.Scripting.API;

/// <summary>
/// The main Manila Build System API
/// </summary>
public static class Manila {
	/// <summary>
	/// Creates a new file handle
	/// </summary>
	/// <param name="path">the path to the file</param>
	/// <returns>the file handle</returns>
	public static ManilaFile file(params string[] path) {
		return new ManilaFile(path);
	}
	/// <summary>
	/// Copies a file handle
	/// </summary>
	/// <param name="file">the original file handle</param>
	/// <returns></returns>
	public static ManilaFile file(ManilaFile file) {
		return new ManilaFile(file.path);
	}

	/// <summary>
	/// Creates a new directory handle
	/// </summary>
	/// <param name="path">the path to the directory</param>
	/// <returns>the directory handle</returns>
	public static ManilaDirectory dir(params string[] path) {
		return new ManilaDirectory(path);
	}
	/// <summary>
	/// Creates a new directory handle
	/// </summary>
	/// <param name="path">the path to the directory</param>
	/// <returns>the directory handle</returns>
	public static ManilaDirectory directory(params string[] path) {
		return new ManilaDirectory(path);
	}
	/// <summary>
	/// Copies a directory handle
	/// </summary>
	/// <param name="dir">the original directory handle</param>
	/// <returns>the directory handle</returns>
	public static ManilaDirectory dir(ManilaDirectory dir) {
		return new ManilaDirectory(dir.path);
	}
	/// <summary>
	/// Creates a new directory handle
	/// </summary>
	/// <param name="dir">the original directory handle</param>
	/// <returns>the directory handle</returns>
	public static ManilaDirectory directory(ManilaDirectory dir) {
		return new ManilaDirectory(dir.path);
	}

	/// <summary>
	/// Gets the current project
	/// </summary>
	public static Project getProject() { return (Project) ScriptManager.currentScriptInstance; }
	/// <summary>
	/// Gets the current workspace
	/// </summary>
	public static Workspace getWorkspace() { return ScriptManager.workspace; }
	/// <summary>
	/// Gets the build configuration
	/// </summary>
	public static object getConfig() { return ScriptManager.buildConfig; }

	/// <summary>
	/// Adds a project filter using a string filter
	/// </summary>
	/// <param name="filter">The string filter to apply</param>
	/// <param name="func">The script function to execute</param>
	public static void project(string filter, ScriptObject func) {
		if (ScriptManager.scope != ScriptManager.Scope.WORKSPACE) throw new Exception("Function 'project' is only available in the workspace scope!");
		ScriptManager.workspace.addProjectFilter(new ProjectFilter.SpecificFilter(filter, func));
	}
	/// <summary>
	/// Adds a project filter using a regular expression filter
	/// </summary>
	/// <param name="filter">The regular expression filter to apply</param>
	/// <param name="func">The script function to execute</param>
	public static void project(Regex filter, ScriptObject func) {
		if (ScriptManager.scope != ScriptManager.Scope.WORKSPACE) throw new Exception("Function 'project' is only available in the workspace scope!");
		ScriptManager.workspace.addProjectFilter(new ProjectFilter.RegexFilter(filter, func));
	}
	/// <summary>
	/// Adds a project filter using a string[] filter to allow for multiple projects
	/// </summary>
	/// <param name="filter">The script object filter to apply</param>
	/// <param name="func">The script function to execute</param>
	public static void project(ScriptObject filter, ScriptObject func) {
		if (ScriptManager.scope != ScriptManager.Scope.WORKSPACE) throw new Exception("Function 'project' is only available in the workspace scope!");
		ScriptManager.workspace.addProjectFilter(new ProjectFilter.ArrayFilter(ScriptUtils.toArray<string>(filter), func));
	}

	/// <summary>
	/// Prints text to stdout
	/// </summary>
	/// <param name="text">the text to print</param>
	public static void print(params dynamic[] text) {
		Functions.print(string.Join(" ", text));
	}
	/// <summary>
	/// Prints text to stdout (with markup support (visit: https://spectreconsole.net/markup))
	/// </summary>
	/// <param name="text">the text to print</param>
	public static void markup(params dynamic[] text) {
		Functions.markup(string.Join(" ", text));
	}
	/// <summary>
	/// Prints text to stdout
	/// </summary>
	/// <param name="text">the text to print</param>
	public static void println(params dynamic[] text) {
		Functions.println(string.Join(" ", text));
	}
	/// <summary>
	/// Prints text to stdout (with markup support (visit: https://spectreconsole.net/markup))
	/// </summary>
	/// <param name="text">the text to print</param>
	public static void markupln(params dynamic[] text) {
		Functions.markupln(string.Join(" ", text));
	}

	public static long taskDuration() {
		return DateTimeOffset.Now.ToUnixTimeMilliseconds() - Task.timeTaskStarted;
	}
	public static long totalTaskDuration() {
		return DateTimeOffset.Now.ToUnixTimeMilliseconds() - Task.timeFirstTaskStarted;
	}
	public static long buildDuration() {
		return DateTimeOffset.Now.ToUnixTimeMilliseconds() - ScriptManager.timeBuildStarted;
	}

	public static Application application(ManilaFile binary) { return new Application(binary); }
	public static FileSet fileSet(ManilaDirectory root) { return new FileSet(root.getPath()); }
}
