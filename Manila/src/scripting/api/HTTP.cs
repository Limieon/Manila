
using System.Net.Http.Json;
using System.Text;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Newtonsoft.Json;
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

	public object post(string url, ScriptObject body) {
		var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
		var reqTask = client.PostAsync(url, content);
		reqTask.Wait();

		var reqTaskRes = reqTask.Result;
		var task = reqTaskRes.Content.ReadAsStringAsync();
		task.Wait();
		return task.Result;
	}

	public object put(string url, ScriptObject body) {
		var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
		var reqTask = client.PutAsync(url, content);
		reqTask.Wait();

		var reqTaskRes = reqTask.Result;
		var task = reqTaskRes.Content.ReadAsStringAsync();
		task.Wait();
		return task.Result;
	}

	public object delete(string url) {
		var reqTask = client.DeleteAsync(url);
		reqTask.Wait();

		var reqTaskRes = reqTask.Result;
		var task = reqTaskRes.Content.ReadAsStringAsync();
		task.Wait();
		return task.Result;
	}
}
