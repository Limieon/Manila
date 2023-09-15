
using Manila.Core;
using Manila.Scripting.Exceptions;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace Manila.Scripting {
	/// <summary>
	/// The main class that connects JS to C#
	/// </summary>
	public class ScriptEngine {
		internal readonly V8ScriptEngine engine;

		internal ScriptEngine() {
			engine = new V8ScriptEngine();
			engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;
			engine.AddHostTypes(typeof(API.Manila));
			API.Functions.addToEngine(engine);
		}
		internal void shutdown() {
			engine.Dispose();
		}

		internal void run(string file) {
			engine.ExecuteDocument(file, ModuleCategory.Standard);
		}
	}
}
