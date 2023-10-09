
using Microsoft.ClearScript;
using Newtonsoft.Json.Linq;

namespace Manila.Scripting.API;

public class HTTP {
	private HttpClient client = new HttpClient();

	public object get(string url) {
		var reqTask = client.GetAsync(url);
		reqTask.Wait();
		var reqTaskRes = reqTask.Result;
		reqTaskRes.EnsureSuccessStatusCode();

		var task = reqTaskRes.Content.ReadAsStringAsync();
		task.Wait();
		return JObject.Parse(task.Result);
	}
}
