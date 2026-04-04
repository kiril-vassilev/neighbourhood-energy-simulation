using System.Text.Json;
using Simulation.DAL;

namespace Simulation.REPORT;

public static class HistorySummaryReport
{
	public static void WriteTo(TextWriter writer, IReadOnlyList<HistoryRow> rows, int stepMinutes, double batteryCapacityKwh)
	{
		writer.WriteLine("Summary report");

		if (rows.Count == 0)
		{
			writer.WriteLine("No history records found.");
			return;
		}

		var stats = ReportHelper.ComputeStats(rows, stepMinutes, batteryCapacityKwh);
		ReportHelper.WriteStatsTo(writer, stats);
	}

	public static string GetJson(IReadOnlyList<HistoryRow> rows, int stepMinutes, double batteryCapacityKwh)
	{
		if (rows.Count == 0)
			return "{}";

		var stats = ReportHelper.ComputeStats(rows, stepMinutes, batteryCapacityKwh);
		return JsonSerializer.Serialize(stats);
	}
}