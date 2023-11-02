
using System.Text;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
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
	/// <param name="headers">Optional request headers</param>
	/// <returns>The response</returns>
	public object get(string url, ScriptObject? headers = null) {
		return convertResponse(client.Send(createRequest(url, HttpMethod.Get, headers, null)));
	}
	/// <summary>
	/// Sends an HTTP Post-Request
	/// </summary>
	/// <param name="url">The url</param>
	/// <param name="body">The body</param>
	/// <param name="headers">Optional request headers</param>
	/// <returns>The response</returns>
	public object post(string url, ScriptObject body, ScriptObject? headers = null) {
		return convertResponse(client.Send(createRequest(url, HttpMethod.Post, headers, null)));
	}
	/// <summary>
	/// Sends a HTTP Put-Request
	/// </summary>
	/// <param name="url">The url</param>
	/// <param name="body">The body</param>
	/// <param name="headers">Optional request headers</param>
	/// <returns>The response</returns>
	public object put(string url, ScriptObject body, ScriptObject? headers = null) {
		return convertResponse(client.Send(createRequest(url, HttpMethod.Put, headers, body)));
	}
	/// <summary>
	/// Sends a HTTP Patch-Request
	/// </summary>
	/// <param name="url">The url</param>
	/// <param name="body">The body</param>
	/// <param name="headers">Optional request headers</param>
	/// <returns>The response</returns>
	public object patch(string url, ScriptObject body, ScriptObject? headers = null) {
		return convertResponse(client.Send(createRequest(url, HttpMethod.Patch, headers, body)));
	}
	/// <summary>
	/// Sends a HTTP Delete-Request
	/// </summary>
	/// <param name="url">The url</param>
	/// <param name="headers">Optional request headers</param>
	/// <returns>The response</returns>
	public object delete(string url, ScriptObject? headers = null) {
		return convertResponse(client.Send(createRequest(url, HttpMethod.Delete, headers, null)));
	}

	private HttpRequestMessage createRequest(string url, HttpMethod method, ScriptObject? headers = null, ScriptObject? body = null) {
		var request = new HttpRequestMessage(method, url);
		if (headers != null) {
			foreach (var key in headers.PropertyNames)
				request.Headers.Add(key, (string) headers.GetProperty(key));
		}
		if (body != null && (method != HttpMethod.Get)) {
			request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
		}
		return request;
	}
	private object convertResponse(HttpResponseMessage m) {
		var task = m.Content.ReadAsStringAsync();
		task.Wait();
		return JObject.Parse(task.Result);
	}
}
