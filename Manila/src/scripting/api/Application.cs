
using System.Diagnostics;
using Microsoft.ClearScript.JavaScript;

namespace Manila.Scripting.API;

public class Application {
	public readonly ManilaFile binary;

	internal Application(ManilaFile binary) {
		this.binary = binary;
	}

	public int run(params string[] args) { return run(binary.getFileDirHandle(), args); }
	public int run(ManilaDirectory workingDir, params string[] args) {
		ProcessStartInfo i = new ProcessStartInfo(binary.getPath());
		i.WorkingDirectory = workingDir.getPath();
		i.Arguments = string.Join<string>(" ", args);

		var handle = Process.Start(i);
		if (handle == null) {
			throw new Exception("Application could not be started!");
		}

		handle.WaitForExit();
		return handle.ExitCode;
	}
}
