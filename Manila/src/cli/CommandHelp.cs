
namespace Manila.CLI;

/// <summary>
/// Defines a abstract help command
/// </summary>
public abstract class CommandHelp {
	/// <summary>
	/// Print help to the app
	/// </summary>
	/// <param name="a">The app</param>
	public abstract void printHelp(App a);
	/// <summary>
	/// Print help to a command inside the app
	/// </summary>
	/// <param name="c">The command</param>
	public abstract void printHelp(Command c);
}

