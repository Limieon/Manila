
namespace Manila.CLI;

public abstract class Command {
	public string name { get; private set; }
	public string description { get; private set; }
	public List<Parameter> parameters { get; private set; }
	public Dictionary<string, Option> options { get; private set; }

	public Command(string name, string description) {
		this.name = name;
		this.description = description;
		this.options = new Dictionary<string, Option>();
		this.parameters = new List<Parameter>();
	}

	public Command addParameter(Parameter parameter) {
		parameters.Add(parameter);
		return this;
	}
	public Command addOption(Option option) {
		options.Add(option.name, option);
		return this;
	}

	public abstract void onExecute(Dictionary<string, object> parameters, Dictionary<string, object> options);
}
