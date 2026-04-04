using Simulation.DAL;

namespace Simulation.REPORT;

public static class HistorySummaryReport
{
	public static void WriteTo(TextWriter writer, IReadOnlyList<HistoryRow> rows)
	{
		writer.WriteLine("Summary report");

		if (rows.Count == 0)
		{
			writer.WriteLine("No history records found.");
			return;
		}

		var stats = ReportHelper.ComputeStats(rows);
		ReportHelper.WriteStatsTo(writer, stats);
	}
}