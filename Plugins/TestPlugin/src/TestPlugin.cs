
using Manila.Plugin.API;

namespace TestPlugin;

public class TestPlugin : Plugin {
	public static TestPlugin instance { get; private set; }

	public TestPlugin() : base("Test Plugin", "A test plugin", "1.0.0") {
		instance = this;
	}

	public override void init() {
		markup("Initializing [purple]Test Plugin[/]...");
	}
}
