
using System.Diagnostics;

namespace Manila.Scripting.API;

public class Application {
	public readonly ManilaFile binary;

	internal Application(ManilaFile binary) {
		this.binary = binary;
	}

	/// <summary>
	/// Starts an application and waits until it exits
	/// </summary>
	/// <param name="args">The arguments that get passed to the application</param>
	/// <returns>The exit code of the application</returns>
	public int run(params string[] args) { return run(binary.getFileDirHandle(), args); }
	/// <summary>
	/// Starts an application and waits until it exits
	/// </summary>
	/// <param name="workingDir">The working directory for the process</param>
	/// <param name="args">The arguments that get passed to the application</param>
	/// <returns>The exit code of the application</returns>
	/// <exception cref="Exception"></exception>
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
