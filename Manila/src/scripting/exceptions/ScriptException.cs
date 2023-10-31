
namespace Manila.Scripting.Exceptions;

/// <summary>
/// A basic script runtime exception
/// </summary>
public class ScriptRuntimeException : Exception {
	/// <summary>
	/// Error hint
	/// </summary>
	public readonly string msg;

	public ScriptRuntimeException(string msg) : base($"A runtime exception occured while executing script! (Error: {msg})") {
		this.msg = msg;
	}
}
