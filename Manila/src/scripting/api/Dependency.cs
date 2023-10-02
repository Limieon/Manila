
using Manila.Utils;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace Manila.Scripting.API;

public static class Dependency {
	public abstract class DependencyResolver {
		public abstract void resolve();
	}
	public class DependencyResolverProject : DependencyResolver {
		public readonly string id;
		public DependencyResolverProject(string id) { this.id = id; }

		public override void resolve() {
			Console.WriteLine("Loading project " + id);
		}
	}

	public static void init(V8ScriptEngine e) {
		e.AddHostObject("dependencies", dependencies);
		e.AddHostObject("prj", prj);
	}

	public static void dependencies(ScriptObject obj) {
		var arr = ScriptUtils.toArray<DependencyResolver>(obj);
		foreach (var resolver in arr) {
			resolver.resolve();
		}
	}
	public static DependencyResolver prj(string id) {
		return new DependencyResolverProject(id);
	}
}
