using System.Text.Json;
using Simulation.DAL;

namespace Simulation.REPORT;

public static class HistorySummaryPerSeasonReport
{
	public static void WriteTo(TextWriter writer, IReadOnlyList<HistoryRow> rows, int stepMinutes, double batteryCapacityKwh)
	{
		writer.WriteLine("Summary per Season report");

		if (rows.Count == 0)
		{
			writer.WriteLine("No history records found.");
			return;
		}

		var groups = rows
			.GroupBy(r => r.Season)
			.OrderBy(g => g.Key);

		foreach (var group in groups)
		{
			writer.WriteLine();
			writer.WriteLine($"--- Season: {group.Key} ---");

			var seasonRows = (IReadOnlyList<HistoryRow>)group.ToList();
			var stats = ReportHelper.ComputeStats(seasonRows, stepMinutes, batteryCapacityKwh);
			ReportHelper.WriteStatsTo(writer, stats);
		}
	}

	public static string GetJson(IReadOnlyList<HistoryRow> rows, int stepMinutes, double batteryCapacityKwh)
	{
		if (rows.Count == 0)
			return "{}";

		var result = rows
			.GroupBy(r => r.Season)
			.OrderBy(g => g.Key)
			.ToDictionary(
				g => g.Key,
				g => ReportHelper.ComputeStats((IReadOnlyList<HistoryRow>)g.ToList(), stepMinutes, batteryCapacityKwh));

		return JsonSerializer.Serialize(result);
	}
}
