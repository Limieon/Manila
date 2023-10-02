
namespace Manila.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a property is not found in a project
/// </summary>
public class NoScriptEnvException : Exception {
	public NoScriptEnvException() : base("No script environment!") { }
}
