
using Manila.Utils;

namespace Manila.Data;

public class Workspace {
	public class Data {
		public string name = "ManilaWorkspace";
		public List<string> authors = new List<string>();
		public List<string> projects = new List<string>();
	}

	public readonly Data data;

	public Workspace() {
		var f = FileUtils.workspaceFile;
		if (!f.exists()) { data = new Data(); return; }
		data = f.deserializeJSON<Data>();
	}

	public void write() {
		FileUtils.workspaceFile.serializeJSON(data, true);
	}
}
