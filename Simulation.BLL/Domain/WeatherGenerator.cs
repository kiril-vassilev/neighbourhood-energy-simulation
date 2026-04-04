// =============================
// Domain/WeatherGenerator.cs
// =============================
namespace Simulation.BLL.Domain;

public static class WeatherGenerator
{
    private static double _winterBaseTemp = 5;
    private static double _springBaseTemp = 12;
    private static double _summerBaseTemp = 22;
    private static double _fallBaseTemp = 14;

    public static void Configure(double winterBaseTemp, double springBaseTemp, double summerBaseTemp, double fallBaseTemp)
    {
        _winterBaseTemp = winterBaseTemp;
        _springBaseTemp = springBaseTemp;
        _summerBaseTemp = summerBaseTemp;
        _fallBaseTemp = fallBaseTemp;
    }

    public static Weather Generate(DateTime time)
    {
        int month = time.Month;

        double baseTemp = month switch
        {
            12 or 1 or 2 => _winterBaseTemp,
            3 or 4 or 5 => _springBaseTemp,
            6 or 7 or 8 => _summerBaseTemp,
            _ => _fallBaseTemp
        };

        return new Weather
        {
            Temperature = baseTemp,
            SolarFactor = Math.Max(0, Math.Sin((time.Hour - 6) / 12.0 * Math.PI))
        };
    }
}