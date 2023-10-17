
using Manila.Utils;
using Microsoft.ClearScript;

namespace Manila.Core;

public static class EventSystem {
	public class Listener {
		public Listener() { }
		public virtual void on() { }
	}

	public class ScriptListener : Listener {
		private ScriptObject func;

		public ScriptListener(ScriptObject func) { this.func = func; }
		public override void on() { func.InvokeAsFunction(); }
	}

	private static Dictionary<string, List<Listener>> listeners = new Dictionary<string, List<Listener>>();

	public static void addListener(string e, Listener l) {
		if (!listeners.ContainsKey(e)) listeners.Add(e, new List<Listener>());
		listeners[e].Add(l);
	}

	public static void fire(string e) {
		Logger.debug($"Executing {listeners[e].Count} listener functions for {e}...");
		foreach (var l in listeners[e]) l.on();
	}
}
