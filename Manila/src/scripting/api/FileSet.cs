
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.VisualBasic;

namespace Manila.Scripting.API;

public class FileSet {
	public readonly string root;
	private readonly Matcher matcher;

	public FileSet(string root) {
		this.root = root;
		matcher = new Matcher(StringComparison.Ordinal);
	}

	public FileSet include(string path) {
		matcher.AddInclude(path);
		return this;
	}
	public FileSet exclude(string path) {
		matcher.AddExclude(path);
		return this;
	}

	public ManilaFile[] files() {
		List<ManilaFile> res = new List<ManilaFile>();
		foreach (var f in matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(root))).Files) {
			res.Add(new ManilaFile(root, f.Path));
		}

		return res.ToArray();
	}
}
