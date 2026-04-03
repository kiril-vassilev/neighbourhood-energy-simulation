// =============================
// Core/SimulationEngine.cs
// =============================

using Simulation.BLL.Domain;
using Simulation.DAL;


namespace Simulation.BLL.Core;

public class SimulationEngine
{
    public SimulationClock Clock { get; }
    public Neighbourhood Neighbourhood { get; }

    public bool DatabaseEnabled { set; get; }

    public Weather CurrentWeather => WeatherGenerator.Generate(Clock.CurrentTime);

    public SimulationEngine(SimulationClock clock, Neighbourhood neighbourhood, bool databaseEnabled)
    {
        Clock = clock;
        Neighbourhood = neighbourhood;
        DatabaseEnabled = databaseEnabled;
    }

    public void Step(bool verbose = true)
    {
        var context = new SimulationContext
        {
            Time = Clock.CurrentTime,
            StepHours = Clock.StepSize.TotalHours,
            Weather = WeatherGenerator.Generate(Clock.CurrentTime)
        };

        Neighbourhood.Update(context);

        var historyRow = new HistoryRow(
            0,
            context.Time,
            Neighbourhood.CurrentLoadKw,
            Clock.Season,
            context.Weather.Temperature,
            Neighbourhood.CurrentLoadWithBatteryKw,
            Neighbourhood.Battery?.CurrentPowerKw ?? 0,
            Neighbourhood.Battery?.StateOfChargeKWh ?? 0,
            Neighbourhood.TotalEnergyKWh,
            Neighbourhood.PeakWithoutBattery,
            Neighbourhood.PeakWithBattery);        

        if (DatabaseEnabled)
            HistoryRepository.Insert(historyRow);

        if (verbose) 
        {
            Console.Clear();
            Console.WriteLine($"Time: {historyRow.CurrentTime}");
            Console.WriteLine($"Season: {historyRow.Season}");
            Console.WriteLine($"Temp: {historyRow.Temperature:F1}C");
            Console.WriteLine($"Load: {historyRow.CurrentLoadKw:F2} kW");

            Console.WriteLine($"Load (With Battery): {historyRow.CurrentLoadWithBatteryKw:F2} kW");
            Console.WriteLine($"Battery Power: {historyRow.BatteryCurrentPowerKw:F2} kW");
            Console.WriteLine($"Battery SoC: {historyRow.BatteryStateOfChargeKwh:F2} kWh");
            Console.WriteLine($"Total Energy: {historyRow.TotalEnergyKwh:F2} kWh");
            
            Console.WriteLine($"Peak Load (Without Battery): {historyRow.PeakWithoutBatteryKwh:F2} kW");
            Console.WriteLine($"Peak Load (With Battery): {historyRow.PeakWithBatteryKwh:F2} kW");

        }

        Clock.Tick();
    }
}

