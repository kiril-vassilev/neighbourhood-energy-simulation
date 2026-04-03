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

		var first = rows[0];
		var last = rows[^1];

		double stepHours = InferStepHours(rows);
		double totalEnergyWithoutBatteryKwh = rows.Sum(r => r.CurrentLoadKw * stepHours);
		double totalEnergyWithBatteryKwh = rows.Sum(r => r.CurrentLoadWithBatteryKw * stepHours);
		double batteryThroughputKwh = rows.Sum(r => Math.Abs(r.BatteryCurrentPowerKw) * stepHours);

		double peakWithoutBatteryKw = rows.Max(r => r.CurrentLoadKw);
		double peakWithBatteryKw = rows.Max(r => r.CurrentLoadWithBatteryKw);
		double peakReductionKw = peakWithoutBatteryKw - peakWithBatteryKw;
		double peakReductionPercent = peakWithoutBatteryKw > 0
			? (peakReductionKw / peakWithoutBatteryKw) * 100.0
			: 0;

		writer.WriteLine($"Rows: {rows.Count}");
		writer.WriteLine($"Period: {first.CurrentTime:yyyy-MM-dd HH:mm:ss} -> {last.CurrentTime:yyyy-MM-dd HH:mm:ss}");
		writer.WriteLine($"Estimated step size: {stepHours:F2} h");
		writer.WriteLine($"Total energy without battery: {totalEnergyWithoutBatteryKwh:F2} kWh");
		writer.WriteLine($"Total energy with battery:    {totalEnergyWithBatteryKwh:F2} kWh");
		writer.WriteLine($"Battery throughput energy:    {batteryThroughputKwh:F2} kWh");
		writer.WriteLine($"Peak load without battery:    {peakWithoutBatteryKw:F2} kW");
		writer.WriteLine($"Peak load with battery:       {peakWithBatteryKw:F2} kW");
		writer.WriteLine($"Peak reduction:               {peakReductionKw:F2} kW ({peakReductionPercent:F2}%)");
	}

	private static double InferStepHours(IReadOnlyList<HistoryRow> rows)
	{
		if (rows.Count < 2)
			return 0.25;

		var minPositive = rows
			.Zip(rows.Skip(1), (a, b) => (b.CurrentTime - a.CurrentTime).TotalHours)
			.Where(hours => hours > 0)
			.DefaultIfEmpty(0.25)
			.Min();

		return minPositive;
	}
}