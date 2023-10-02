
namespace Manila.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a directory is not a script environment
/// </summary>
public class NoScriptEnvException : Exception {
	public NoScriptEnvException() : base("No script environment!") { }
}
