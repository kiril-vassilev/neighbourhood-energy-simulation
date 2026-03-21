// =============================
// Domain/WeatherGenerator.cs
// =============================
namespace Simulation.BLL.Domain;

public static class WeatherGenerator
{
    public static Weather Generate(DateTime time)
    {
        int month = time.Month;

        double baseTemp = month switch
        {
            12 or 1 or 2 => 5,
            3 or 4 or 5 => 12,
            6 or 7 or 8 => 22,
            _ => 14
        };

        return new Weather
        {
            Temperature = baseTemp,
            SolarFactor = Math.Max(0, Math.Sin((time.Hour - 6) / 12.0 * Math.PI))
        };
    }
}