
namespace Manila.Utils;

/// <summary>
/// Provides utlity functions for formatting data into strings
/// </summary>
public static class FormattingUtils {
	private static readonly int MS_PER_S = 1000;
	private static readonly int MS_PER_M = 60 * 1000;

	/// <summary>
	/// Converts a duration in milliseconds to a string representation
	/// </summary>
	/// <param name="duration">The duration in milliseconds to be converted</param>
	/// <returns> A string representation of the duration</returns>
	public static string stringifyDuration(long duration) {
		var MS_PER_S = 1000;
		var MS_PER_M = 60 * 1000;

		long minutes = (long) Math.Floor((double) (duration / MS_PER_M));
		duration -= minutes * MS_PER_M;

		long seconds = (long) Math.Floor((double) (duration / MS_PER_S));
		duration -= seconds * MS_PER_S;

		if (seconds < 1 && minutes < 1) return $"{duration}ms";
		if (seconds > 0 && minutes < 1) return $"{seconds}s{duration}ms";
		return $"{minutes}m{seconds}s{duration}ms";
	}
}
