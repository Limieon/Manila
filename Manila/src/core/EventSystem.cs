
using Manila.Utils;
using Microsoft.ClearScript;

namespace Manila.Core;

public class EventSystem {
	public class Listener {
		public Listener() { }
		public virtual void on() { }
	}

	public class ScriptListener : Listener {
		private ScriptObject func;

		public ScriptListener(ScriptObject func) { this.func = func; }
		public override void on() { func.InvokeAsFunction(); }
	}

	private Dictionary<string, List<Listener>> listeners = new Dictionary<string, List<Listener>>();

	public void addListener(string e, Listener l) {
		if (!listeners.ContainsKey(e)) listeners.Add(e, new List<Listener>());
		listeners[e].Add(l);
	}

	public void fire(string e) {
		if (!listeners.ContainsKey(e)) return;
		Logger.debug($"Executing {listeners[e].Count} listener functions for {e}...");
		foreach (var l in listeners[e]) l.on();
	}

	public void on(string e, ScriptObject func) {
		if (!listeners.ContainsKey(e)) listeners.Add(e, new List<Listener>());
		listeners[e].Add(new ScriptListener(func));
	}
}
