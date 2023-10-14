
namespace Manila.Scripting.API;

/// <summary>
/// Simple time api
/// </summary>
public class Time {
	internal Time() { }

	/// <summary>
	/// Gets current unix timestamp
	/// </summary>
	/// <returns>Time in ms</returns>
	public long now() { return DateTimeOffset.Now.ToUnixTimeMilliseconds(); }
}
