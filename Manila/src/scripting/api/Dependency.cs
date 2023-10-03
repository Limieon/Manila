
using Manila.Utils;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace Manila.Scripting.API;

public static class Dependency {
	public abstract class Resolver {
		public abstract void resolve();
	}
	public class ResolverProject : Resolver {
		public readonly string id;
		public ResolverProject(string id) { this.id = id; }

		public override void resolve() {
			Console.WriteLine("Loading project " + id);
		}
	}
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

	public static void init(V8ScriptEngine e) {
		e.AddHostObject("dependencies", dependencies);
		e.AddHostObject("project", project);
	}

	public static void dependencies(ScriptObject obj) {
		var arr = ScriptUtils.toArray<Resolver>(obj);
		foreach (var resolver in arr) {
			resolver.resolve();
		}
	}
	public static Resolver project(string id) {
		return new ResolverProject(id);
	}
	public static Resolver external(string projectNamespace, string projectID) {
		return new ResolverExternal(projectNamespace, projectID);
	}
}
