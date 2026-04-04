using Simulation.DAL;

namespace Simulation.REPORT;

public static class HistorySummaryPerSeasonReport
{
	public static void WriteTo(TextWriter writer, IReadOnlyList<HistoryRow> rows)
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
			var stats = ReportHelper.ComputeStats(seasonRows);
			ReportHelper.WriteStatsTo(writer, stats);
		}
	}
}
