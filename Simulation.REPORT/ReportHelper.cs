using Simulation.DAL;

namespace Simulation.REPORT;

public sealed record ReportStats(
	int RowCount,
	DateTime FirstTime,
	DateTime LastTime,
	double StepHours,
	double TotalEnergyWithoutBatteryKwh,
	double TotalEnergyWithBatteryKwh,
	double BatteryThroughputKwh,
    double TotalBateryChargedKwh,
    double TotalBatteryDischargedKwh,
	double PeakWithoutBatteryKw,
	double PeakWithBatteryKw,
	double AverageWithoutBatteryKw,
	double AverageWithBatteryKw,
	double StdDevWithoutBatteryKw,
	double StdDevWithBatteryKw,
	double HighPeakHoursPerMonth,
	double BatteryCapacityKwh,
	double BatteryUtilizationPercent,
	double PeakReductionKw,
	double PeakReductionPercent);

public static class ReportHelper
{
	public static double InferStepHours(IReadOnlyList<HistoryRow> rows)
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

    public static double HighPeakHoursPerMonth_WithoutBattery(IReadOnlyList<HistoryRow> rows, double stepHours = 0.25)
    {
        // Top 10% threshold
        double threshold = MathHelper.Percentile(rows.Select(r => r.CurrentLoadKw).ToList(), 0.95);

        var highPeaks = rows
            .Where(r => r.CurrentLoadKw >= threshold)
            .ToList();

        int totalMonths = rows
            .Select(r => new { r.CurrentTime.Year, r.CurrentTime.Month })
            .Distinct()
            .Count();

        double highPeakHoursPerMonth = totalMonths > 0
            ? (double)highPeaks.Count / totalMonths
            : 0;

        return highPeakHoursPerMonth * stepHours;
    }

    public static double BatteryUtlizationPercent(IReadOnlyList<HistoryRow> rows, double totalBatteryDischargeKwh, double batteryCapacityKwh, double stepHours = 0.25)
    {
        int days = rows
            .Select(r => r.CurrentTime.Date)
            .Distinct()
            .Count();

        if (batteryCapacityKwh <= 0 || days == 0)
            return 0;            

        return  totalBatteryDischargeKwh > 0 
            ? totalBatteryDischargeKwh / (batteryCapacityKwh * days) * 100.0
            : 0;
    }    

	public static ReportStats ComputeStats(IReadOnlyList<HistoryRow> rows, int stepMinutes, double batteryCapacityKwh)
	{
		if (rows.Count == 0)
		{
			return new ReportStats(
				RowCount: 0,
				FirstTime: DateTime.MinValue,
				LastTime: DateTime.MinValue,
				StepHours: 0.25,
				TotalEnergyWithoutBatteryKwh: 0,
				TotalEnergyWithBatteryKwh: 0,
				BatteryThroughputKwh: 0,
                TotalBateryChargedKwh: 0,
                TotalBatteryDischargedKwh: 0,
				PeakWithoutBatteryKw: 0,
				PeakWithBatteryKw: 0,
				AverageWithoutBatteryKw: 0,
				AverageWithBatteryKw: 0,
				StdDevWithoutBatteryKw: 0,
				StdDevWithBatteryKw: 0,
				HighPeakHoursPerMonth: 0,
				BatteryCapacityKwh: batteryCapacityKwh,
				BatteryUtilizationPercent: 0,
				PeakReductionKw: 0,
				PeakReductionPercent: 0);
		}

        double stepHours = stepMinutes / 60.0;


		double totalEnergyWithoutBatteryKwh = rows.Sum(r => r.CurrentLoadKw * stepHours);
		double totalEnergyWithBatteryKwh = rows.Sum(r => r.CurrentLoadWithBatteryKw * stepHours);
		double batteryThroughputKwh = rows.Sum(r => Math.Abs(r.BatteryCurrentPowerKw) * stepHours);

        double totalBatteryChargedKwh = rows
            .Where(r => r.BatteryCurrentPowerKw > 0)
            .Sum(r => r.BatteryCurrentPowerKw * stepHours);

        double totalBatteryDischargedKwh = rows
            .Where(r => r.BatteryCurrentPowerKw < 0)
            .Sum(r => -r.BatteryCurrentPowerKw * stepHours);

		double peakWithoutBatteryKw = rows.Max(r => r.CurrentLoadKw);
		double peakWithBatteryKw = rows.Max(r => r.CurrentLoadWithBatteryKw);

		double averageWithoutBatteryKw = rows.Average(r => r.CurrentLoadKw);
		double averageWithBatteryKw = rows.Average(r => r.CurrentLoadWithBatteryKw);

		double stdDevWithoutBatteryKw = Math.Sqrt(rows.Average(r => Math.Pow(r.CurrentLoadKw - averageWithoutBatteryKw, 2)));
		double stdDevWithBatteryKw = Math.Sqrt(rows.Average(r => Math.Pow(r.CurrentLoadWithBatteryKw - averageWithBatteryKw, 2)));

		double highPeakHoursPerMonth = HighPeakHoursPerMonth_WithoutBattery(rows, stepHours);
		double batteryUtilizationPercent =BatteryUtlizationPercent(rows, totalBatteryDischargedKwh, batteryCapacityKwh, stepHours);

		double peakReductionKw = peakWithoutBatteryKw - peakWithBatteryKw;
		double peakReductionPercent = peakWithoutBatteryKw > 0
			? (peakReductionKw / peakWithoutBatteryKw) * 100.0
			: 0;

		return new ReportStats(
			RowCount: rows.Count,
			FirstTime: rows[0].CurrentTime,
			LastTime: rows[^1].CurrentTime,
			StepHours: stepHours,
			TotalEnergyWithoutBatteryKwh: totalEnergyWithoutBatteryKwh,
			TotalEnergyWithBatteryKwh: totalEnergyWithBatteryKwh,
			BatteryThroughputKwh: batteryThroughputKwh,
            TotalBateryChargedKwh: totalBatteryChargedKwh,
            TotalBatteryDischargedKwh: totalBatteryDischargedKwh,
			PeakWithoutBatteryKw: peakWithoutBatteryKw,
			PeakWithBatteryKw: peakWithBatteryKw,
			AverageWithoutBatteryKw: averageWithoutBatteryKw,
			AverageWithBatteryKw: averageWithBatteryKw,
			StdDevWithoutBatteryKw: stdDevWithoutBatteryKw,
			StdDevWithBatteryKw: stdDevWithBatteryKw,
			HighPeakHoursPerMonth: highPeakHoursPerMonth,
			BatteryCapacityKwh: batteryCapacityKwh,
			BatteryUtilizationPercent: batteryUtilizationPercent,
			PeakReductionKw: peakReductionKw,
			PeakReductionPercent: peakReductionPercent);
	}

	public static void WriteStatsTo(TextWriter writer, ReportStats s)
	{
		writer.WriteLine($"Rows: {s.RowCount}");
		writer.WriteLine($"Period: {s.FirstTime:yyyy-MM-dd HH:mm:ss} -> {s.LastTime:yyyy-MM-dd HH:mm:ss}");
		writer.WriteLine($"Step size: {s.StepHours:F2} h");
		writer.WriteLine($"Battery capacity: {s.BatteryCapacityKwh:F2} kWh");
		writer.WriteLine("---------------------------------------------------------------------------");
		writer.WriteLine($"Total energy without battery:    {s.TotalEnergyWithoutBatteryKwh:F2} kWh");
		writer.WriteLine($"Total energy with battery:       {s.TotalEnergyWithBatteryKwh:F2} kWh");
		writer.WriteLine($"Battery throughput energy:       {s.BatteryThroughputKwh:F2} kWh");
        writer.WriteLine($"Total battery charged energy:    {s.TotalBateryChargedKwh:F2} kWh");
        writer.WriteLine($"Total battery discharged energy: {s.TotalBatteryDischargedKwh:F2} kWh");
		writer.WriteLine($"Peak load without battery:       {s.PeakWithoutBatteryKw:F2} kW");
		writer.WriteLine($"Peak load with battery:          {s.PeakWithBatteryKw:F2} kW");
		writer.WriteLine($"Average load without battery:    {s.AverageWithoutBatteryKw:F2} kW");
		writer.WriteLine($"Average load with battery:       {s.AverageWithBatteryKw:F2} kW");
		writer.WriteLine($"Std dev without battery:         {s.StdDevWithoutBatteryKw:F2} kW");
		writer.WriteLine($"Std dev with battery:            {s.StdDevWithBatteryKw:F2} kW");
		writer.WriteLine($"High peak hours per month:       {s.HighPeakHoursPerMonth:F2} h");
		writer.WriteLine($"Battery utilization:             {s.BatteryUtilizationPercent:F2}%");
		writer.WriteLine($"Peak reduction:                  {s.PeakReductionKw:F2} kW ({s.PeakReductionPercent:F2}%)");
	}
}
