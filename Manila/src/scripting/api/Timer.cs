
using Manila.Scripting.API;
using Manila.Utils;

public class APITimer {
	private long timeStarted = 0;
	private long timeStopped;
	private bool stopped = false;

	internal APITimer(bool stopped = false) { if (!stopped) timeStarted = DateTimeOffset.Now.ToUnixTimeMilliseconds(); }

	/// <summary>
	/// Gets the elapsed time
	/// </summary>
	/// <returns>Elapsed time in ms</returns>
	public long get() {
		return stopped ? timeStopped - timeStarted : DateTimeOffset.Now.ToUnixTimeMilliseconds() - timeStarted;
	}

	/// <summary>
	/// Starts / restarts the timer
	/// </summary>
	public void start() {
		timeStarted = DateTimeOffset.Now.ToUnixTimeMilliseconds();
	}
	/// <summary>
	/// Stops the timer and returns elapsed time since start
	/// </summary>
	/// <returns>Elapsed time in ms</returns>
	public long stop() {
		stopped = true;
		timeStopped = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		return DateTimeOffset.Now.ToUnixTimeMilliseconds() - timeStarted;
	}
	/// <summary>
	/// Returns the fromatted time instead of unix time
	/// </summary>
	/// <returns>Format like 1m3s195ms</returns>
	public string getFormatted() { return FormattingUtils.stringifyDuration(get()); }
}
