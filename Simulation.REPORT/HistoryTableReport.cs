using Simulation.DAL;

namespace Simulation.REPORT;

public static class HistoryTableReport
{
	public static void WriteTo(TextWriter writer, IReadOnlyList<HistoryRow> rows)
	{
		if (rows.Count == 0)
		{
			writer.WriteLine("History report");
			writer.WriteLine("No history records found.");
			return;
		}

		writer.WriteLine("History report");
		writer.WriteLine($"Rows: {rows.Count}");
		writer.WriteLine();
		writer.WriteLine(
			"Id | Time                | Season | TempC | LoadKw | LoadWithBatteryKw | BatteryPowerKw | BatterySoCKwh | TotalEnergyKwh | PeakWithoutBatteryKw | PeakWithBatteryKw");
		writer.WriteLine(
			"---+---------------------+--------+-------+--------+-------------------+----------------+----------------+----------------+----------------------+------------------");

		foreach (var row in rows)
		{
			writer.WriteLine(
				$"{row.Id,2} | {row.CurrentTime:yyyy-MM-dd HH:mm:ss} | {row.Season,-6} | {row.Temperature,5:F1} | {row.CurrentLoadKw,6:F2} | {row.CurrentLoadWithBatteryKw,17:F2} | {row.BatteryCurrentPowerKw,14:F2} | {row.BatteryStateOfChargeKwh,14:F2} | {row.TotalEnergyKwh,14:F2} | {row.PeakWithoutBatteryKwh,20:F2} | {row.PeakWithBatteryKwh,16:F2}");
		}
	}
}