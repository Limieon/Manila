
using Manila.Utils;
using Microsoft.ClearScript.V8;

namespace Manila.Scripting.API;

public static class Parameter {
	public enum ParamterType {
		STRING,
		FLAG,
		INT,
		FLOAT,
		ANY
	}
	public record Data(string name, string description, ParamterType type, char? alias = null) { }

	private static Dictionary<string, object> parsedArguments = new Dictionary<string, object>();
	public static Dictionary<string, Data> parameters = new Dictionary<string, Data>();

	public static void init(string[] args, V8ScriptEngine e) {
		parsedArguments = CLIUtils.parseArguments(args);

		e.AddHostObject("parameterInt", parameterInt);
		e.AddHostObject("parameterFloat", parameterFloat);
		e.AddHostObject("parameterString", parameterString);
		e.AddHostObject("parameterFlag", parameterFlag);
	}

	public static T parameter<T>(string name) {
		return (T) parsedArguments[name];
	}

	public static int parameterInt(string name, string description, char? alias = null) {
		parameters.Add(name, new Data(name, description, ParamterType.INT, alias));
		return parameter<int>(name);
	}
	public static float parameterFloat(string name, string description, char? alias = null) {
		parameters.Add(name, new Data(name, description, ParamterType.FLOAT, alias));
		return parameter<float>(name);
	}
	public static string parameterString(string name, string description, char? alias = null) {
		parameters.Add(name, new Data(name, description, ParamterType.STRING, alias));
		return parameter<string>(name);
	}
	public static bool parameterFlag(string name, string description, char? alias = null) {
		parameters.Add(name, new Data(name, description, ParamterType.FLAG, alias));
		return parameter<bool>(name);
	}
}
