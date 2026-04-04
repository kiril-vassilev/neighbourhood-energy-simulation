using Simulation.DAL;


namespace Simulation.REPORT;

public static class MathHelper
{
    public static double Percentile(List<double> sequence, double percentile)
    {
        var sorted = sequence.OrderBy(x => x).ToList();
        double position = (sorted.Count + 1) * percentile;
        int index = (int)position;

        if (index < 1) return sorted.First();
        if (index >= sorted.Count) return sorted.Last();

        double fraction = position - index;
        return sorted[index - 1] + fraction * (sorted[index] - sorted[index - 1]);
    }

    public static double HighPeakHoursPerMonth_WithoutBattery(IReadOnlyList<HistoryRow> rows, double stepHours = 0.25)
    {
        // Top 10% threshold
        double threshold = Percentile(rows.Select(r => r.CurrentLoadKw).ToList(), 0.95);

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

    public static double StandardDeviation(List<double> sequence)
    {
        double average = sequence.Average();
        double sumOfSquares = sequence.Sum(x => Math.Pow(x - average, 2));
        return Math.Sqrt(sumOfSquares / sequence.Count);
    }

    public static double BatteryUtlizationPercent(IReadOnlyList<HistoryRow> rows, double stepHours = 0.25)
    {
        double totalEnergyWithoutBattery = rows.Sum(r => r.CurrentLoadKw * stepHours);
        double totalEnergyWithBattery = rows.Sum(r => r.CurrentLoadWithBatteryKw * stepHours);
        double energyShifted = totalEnergyWithoutBattery - totalEnergyWithBattery;

        return totalEnergyWithoutBattery > 0
            ? (energyShifted / totalEnergyWithoutBattery) * 100.0
            : 0;
    }
}