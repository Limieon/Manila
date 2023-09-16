
namespace Manila.CLI.Exceptions;

internal class OptionProvidedNotFoundExceptions : Exception {
	public Command command { get; private set; }
	public string option { get; private set; }

	public OptionProvidedNotFoundExceptions(Command c, string option) : base("Option with name '" + option + "' provided but not found on command '" + c.name + "'!") {
		this.command = c;
		this.option = option;
	}
}
