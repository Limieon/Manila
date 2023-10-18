
using Manila.Core;
using Manila.Utils;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace Manila.Scripting.API;

/// <summary>
/// API for dependency management
/// </summary>
public static class Dependency {
	/// <summary>
	/// Base class for dependency resolvers
	/// </summary>
	public abstract class Resolver {
		public abstract Project? resolve();
	}
	/// <summary>
	/// Project dependency resolver
	/// </summary>
	public class ResolverProject : Resolver {
		public readonly string prjID;
		internal ResolverProject(string prjID) { this.prjID = prjID; }

		public override Project? resolve() {
			return ScriptManager.workspace.getProject(prjID);
		}
	}
	/// <summary>
	/// External dependency resolver
	/// </summary>
	public class ResolverExternal : Resolver {
		public readonly string projectID;
		public readonly string projectNamespace;

		public ResolverExternal(string projectNamespace, string projectID) {
			this.projectNamespace = projectNamespace;
			this.projectID = projectID;
		}

		public override Project? resolve() {
			Console.WriteLine("Loading external dependency '" + projectNamespace + "':'" + projectID + "'");
			return null;
		}
	}

	internal static void init(V8ScriptEngine e) {
		e.AddHostObject("dependencies", dependencies);
		e.AddHostObject("project", project);
	}

	public static void dependencies(ScriptObject obj) {
		ScriptManager.currentScriptInstance.depndencyResolvers.AddRange(ScriptUtils.toArray<Resolver>(obj));
	}
	public static Resolver project(string prjID) {
		return new ResolverProject(prjID);
	}
	public static Resolver external(string projectNamespace, string projectID) {
		return new ResolverExternal(projectNamespace, projectID);
	}
}
