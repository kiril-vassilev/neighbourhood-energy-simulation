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

    public static double StandardDeviation(List<double> sequence)
    {
        double average = sequence.Average();
        double sumOfSquares = sequence.Sum(x => Math.Pow(x - average, 2));
        return Math.Sqrt(sumOfSquares / sequence.Count);
    }
}