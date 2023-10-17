
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
		public abstract void resolve();
	}
	/// <summary>
	/// Project dependency resolver
	/// </summary>
	public class ResolverProject : Resolver {
		public readonly string id;
		internal ResolverProject(string id) { this.id = id; }

		public override void resolve() {
			Console.WriteLine("Loading project " + id);
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

		public override void resolve() {
			Console.WriteLine("Loading external dependency '" + projectNamespace + "':'" + projectID + "'");
		}
	}

	internal static void init(V8ScriptEngine e) {
		e.AddHostObject("dependencies", dependencies);
		e.AddHostObject("project", project);
	}

	public static void dependencies(ScriptObject obj) {
		ScriptManager.currentScriptInstance.depndencyResolvers.AddRange(ScriptUtils.toArray<Resolver>(obj));
	}
	public static Resolver project(string id) {
		return new ResolverProject(id);
	}
	public static Resolver external(string projectNamespace, string projectID) {
		return new ResolverExternal(projectNamespace, projectID);
	}
}
