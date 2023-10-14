
using System.Globalization;

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
	public string formatted(string format = "MM/dd/yyyy - HH:mm:ss") { return DateTimeOffset.Now.ToString(format); }
	public string zone() {
		string wtzID = TimeZoneInfo.Local.Id;
		string ianiatzID;
		if (TimeZoneInfo.TryConvertWindowsIdToIanaId(wtzID, RegionInfo.CurrentRegion.TwoLetterISORegionName, out ianiatzID)) return ianiatzID;
		return wtzID;
	}
}
