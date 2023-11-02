
namespace Manila.Utils;

public static class DictUtils {
	public static Dictionary<TKey, TValue> merge<TKey, TValue>(Dictionary<TKey, TValue> a, Dictionary<TKey, TValue> b) {
		var merged = a.Concat(b).ToDictionary(x => x.Key, x => x.Value);
		return merged;
	}
}
