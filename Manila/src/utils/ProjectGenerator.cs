
using Manila.Scripting.API;
using Newtonsoft.Json;

namespace Manila.Utils;

internal class ProjectGenerator {
	internal class WorkspaceData {
		public string name;
		public List<string> authors = new List<string>();
		public List<string> projects = new List<string>();
	}

	public string name { get; private set; }
	public ManilaDirectory dir { get; private set; }

	public ProjectGenerator(string name) {
		this.name = name;
		dir = new ManilaDirectory(Directory.GetCurrentDirectory()).join(name);
		dir.create();
	}

	public void generateWorkspaceFile() {
		var workspace = new WorkspaceData();
		workspace.name = name;

		dir.file("workspace.manila").write(JsonConvert.SerializeObject(workspace));
	}
}
