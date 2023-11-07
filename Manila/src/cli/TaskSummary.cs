
using Manila.Scripting.API;
using Manila.Utils;
using Spectre.Console;

namespace Manila.Core;

public static class TaskSummary {
	public enum TaskStatus {
		RUNNING, SUCCESS, FAILED, ABORTED, SKIPPED
	}

	public class TaskData {
		public readonly Scripting.API.Task task;
		public readonly List<TaskData> subTasks = new List<TaskData>();
		public TaskStatus status { get; private set; }
		public readonly APITimer timer = new APITimer();

		public TaskData(Scripting.API.Task t) {
			task = t;
			status = TaskStatus.RUNNING;
		}

		public void addSubTask(Scripting.API.Task t) {
			subTasks.Add(new TaskData(t));
		}
		public void updateStatus(TaskStatus s) {
			if (s != TaskStatus.RUNNING) timer.stop();
			status = s;
		}
	}

	private static string formatStatus(TaskStatus s) {
		switch (s) {
			case TaskStatus.RUNNING: return "[gray]Running...[/]";
			case TaskStatus.ABORTED: return "[red]Aborted[/]";
			case TaskStatus.FAILED: return "[red]Failed[/]";
			case TaskStatus.SKIPPED: return "[yellow]Skipped[/]";
			case TaskStatus.SUCCESS: return "[green]Success[/]";
		}
		return "";
	}
	private static string formatDuration(APITimer timer) {
		return $"[cyan]{timer.getFormatted()}[/]";
	}
	internal static string formatTaskName(string name, int deep = 0) {
		var res = "";
		for (var i = 0; i < deep - 1; ++i) res += "  ";
		return res + (deep > 0 ? "  " : "") + name;
	}

	internal static readonly List<TaskData> data = new List<TaskData>();

	internal static void taskStarted(Scripting.API.Task t) {
		if (data.Count > 0 && data[data.Count - 1].status == TaskStatus.RUNNING) {
			data[data.Count - 1].addSubTask(t);
			return;
		}
		data.Add(new TaskData(t));
	}
	internal static bool taskEnded(TaskStatus r = TaskStatus.SUCCESS, TaskData? d = null) {
		if (d == null) d = data[data.Count - 1];
		for (var i = d.subTasks.Count - 1; i >= 0; --i) {
			if (taskEnded(r, d.subTasks[i])) return true;
		}

		if (d.status == TaskStatus.RUNNING) {
			d.updateStatus(r); return true;
		}

		return false;
	}

	private static void rec(TaskData d, Table r, int deep) {
		foreach (var t in d.subTasks) {
			r.AddRow(formatTaskName(ScriptUtils.getTaskName(t.task), deep), formatDuration(t.timer), formatStatus(t.status));
			foreach (var t2 in t.subTasks) rec(t2, r, deep++);
		}
	}
	private static Table getResultTable() {
		var res = new Table();
		res.AddColumns("Task", "Duration", "Status");

		foreach (var t in data) {
			res.AddRow(formatTaskName(ScriptUtils.getTaskName(t.task)), formatDuration(t.timer), formatStatus(t.status));
			rec(t, res, 1);
		}

		return res;
	}

	internal static void printResults() {
		var t = getResultTable();
		t.Border(TableBorder.Rounded);
		AnsiConsole.Write(t);
	}
}
