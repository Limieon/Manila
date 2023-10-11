
using System.Text;
using Microsoft.ClearScript;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Manila.Scripting.API;

/// <summary>
/// API for HTTP Requests
/// </summary>
public class HTTP {
	private HttpClient client = new HttpClient();

	/// <summary>
	/// Sends an HTTP Get-Request
	/// </summary>
	/// <param name="url">The url</param>
	/// <returns>The response</returns>
	public object get(string url) {
		var reqTask = client.GetAsync(url);
		reqTask.Wait();
		var reqTaskRes = reqTask.Result;
		reqTaskRes.EnsureSuccessStatusCode();

		var task = reqTaskRes.Content.ReadAsStringAsync();
		task.Wait();
		return JObject.Parse(task.Result);
	}
	/// <summary>
	/// Sends an HTTP Post-Request
	/// </summary>
	/// <param name="url">The url</param>
	/// <param name="body">The body</param>
	/// <returns>The response</returns>
	public object post(string url, ScriptObject body) {
		var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
		var reqTask = client.PostAsync(url, content);
		reqTask.Wait();

		var reqTaskRes = reqTask.Result;
		var task = reqTaskRes.Content.ReadAsStringAsync();
		task.Wait();
		return task.Result;
	}
	/// <summary>
	/// Sends a HTTP Put-Request
	/// </summary>
	/// <param name="url">The url</param>
	/// <param name="body">The body</param>
	/// <returns>The response</returns>
	public object put(string url, ScriptObject body) {
		var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
		var reqTask = client.PutAsync(url, content);
		reqTask.Wait();

		var reqTaskRes = reqTask.Result;
		var task = reqTaskRes.Content.ReadAsStringAsync();
		task.Wait();
		return task.Result;
	}
	/// <summary>
	/// Sends a HTTP Delete-Request
	/// </summary>
	/// <param name="url">The url</param>
	/// <returns>The response</returns>
	public object delete(string url) {
		var reqTask = client.DeleteAsync(url);
		reqTask.Wait();

		var reqTaskRes = reqTask.Result;
		var task = reqTaskRes.Content.ReadAsStringAsync();
		task.Wait();
		return task.Result;
	}
}
