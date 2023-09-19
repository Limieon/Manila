
namespace Manila.CLI;

/// <summary>
/// Represents a CLI command.
/// </summary>
public abstract class Command {
	/// <summary>
	/// Gets the name of the command.
	/// </summary>
	public string name { get; private set; }

	/// <summary>
	/// Gets the description of the command.
	/// </summary>
	public string description { get; private set; }

	/// <summary>
	/// Gets or sets the list of parameters associated with the command.
	/// </summary>
	public List<Parameter> parameters { get; private set; }

	/// <summary>
	/// Gets or sets the dictionary of options associated with the command.
	/// </summary>
	public Dictionary<string, Option> options { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Command"/> class with the specified name and description.
	/// </summary>
	/// <param name="name">The name of the command</param>
	/// <param name="description">The description of the command</param>
	public Command(string name, string description) {
		this.name = name;
		this.description = description;
		options = new Dictionary<string, Option>();
		parameters = new List<Parameter>();
	}

	/// <summary>
	/// Adds a parameter to the command.
	/// </summary>
	/// <param name="parameter">The parameter to add</param>
	/// <returns>The current instance of the command</returns>
	public Command addParameter(Parameter parameter) {
		parameters.Add(parameter);
		return this;
	}

	/// <summary>
	/// Adds an option to the command.
	/// </summary>
	/// <param name="option">The option to add</param>
	/// <returns>The current instance of the command</returns>
	public Command addOption(Option option) {
		options.Add(option.name, option);
		return this;
	}

	/// <summary>
	/// Executes the command with the given parameters and options.
	/// </summary>
	/// <param name="parameters">A dictionary of parameters</param>
	/// <param name="options">A dictionary of options</param>
	public virtual void onExecute(Dictionary<string, object> parameters, Dictionary<string, object> options) { }
	/// <summary>
	/// Executes the command with the given parameters, options, and application context.
	/// </summary>
	/// <param name="parameters">A dictionary of parameters</param>
	/// <param name="options">A dictionary of options</param>
	/// <param name="app">The application context</param>
	public virtual void onExecute(Dictionary<string, object> parameters, Dictionary<string, object> options, App app) { }
}
