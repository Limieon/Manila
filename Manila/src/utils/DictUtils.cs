
using System.Reflection;

namespace Manila.Utils;

public static class DictUtils {
	public static Dictionary<TKey, TValue> merge<TKey, TValue>(Dictionary<TKey, TValue> a, Dictionary<TKey, TValue> b) {
		var merged = a.Concat(b).ToDictionary(x => x.Key, x => x.Value);
		return merged;
	}

	public static Dictionary<string, dynamic> fromFields(object o, string fieldPrefix = "_", bool applyPrefixForKeys = false) {
		var res = new Dictionary<string, dynamic>();
		var prefixLength = fieldPrefix.Length;

		var fields = o.GetType().GetFields();
		foreach (var f in fields) {
			if (f.Name.StartsWith(fieldPrefix)) {
				res.Add(applyPrefixForKeys ? f.Name : f.Name.Substring(prefixLength), f.GetValue(o));
			}
		}

		return res;
	}
}
