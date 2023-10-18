
using System.Text.RegularExpressions;
using Manila.Core;
using Manila.Scripting.API;

namespace Manila.Utils;

public static class PathUtils {
	public static string parse(string path, Workspace workspace) {
		if (path.StartsWith("@")) {
			var groups = new Regex(@"@(.*?)[\/\\](.*)").Match(path).Groups;
			var dirName = groups[1].Value;
			if (!workspace.namedDirectories.ContainsKey(dirName)) throw new ArgumentException($"No named directory called {dirName} found!");
			return new ManilaFile(new ManilaDirectory(workspace.namedDirectories[dirName]), groups[2].Value).getPath();
		}
		return new ManilaDirectory(path).getPath();
	}
}
