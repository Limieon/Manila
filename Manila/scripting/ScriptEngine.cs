
using Microsoft.ClearScript.V8;

namespace Manila.Scripting {
	class ScriptEngine {
		private V8ScriptEngine engine;

		public ScriptEngine() {
			engine = new V8ScriptEngine();
			engine.AddHostType("Console", typeof(Console));
			engine.Execute(File.ReadAllText("./Manila.js"));
		}
	}
}
