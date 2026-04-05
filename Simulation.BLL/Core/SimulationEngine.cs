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
    public HistoryRow? CurrentHistoryRow { get; private set; }

    public Weather CurrentWeather => WeatherGenerator.Generate(Clock.CurrentTime);

    public SimulationEngine(SimulationClock clock, Neighbourhood neighbourhood)
    {
        Clock = clock;
        Neighbourhood = neighbourhood;
    }

    public void Step(bool IsPresenting = true)
    {
        var context = new SimulationContext
        {
            Time = Clock.CurrentTime,
            StepHours = Clock.StepSize.TotalHours,
            Weather = WeatherGenerator.Generate(Clock.CurrentTime)
        };

        Neighbourhood.Update(context);

        CurrentHistoryRow = new HistoryRow(
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

        if (!IsPresenting)

            // In generate-data mode, save history to database
            HistoryRepository.Insert(CurrentHistoryRow);
        else
        {
            // In presenting mode, print to console
            Console.Clear();
            Console.WriteLine($"Time: {CurrentHistoryRow.CurrentTime}");
            Console.WriteLine($"Season: {CurrentHistoryRow.Season}");
            Console.WriteLine($"Temp: {CurrentHistoryRow.Temperature:F1}C");
            Console.WriteLine($"Load: {CurrentHistoryRow.CurrentLoadKw:F2} kW");

            Console.WriteLine($"Load (With Battery): {CurrentHistoryRow.CurrentLoadWithBatteryKw:F2} kW");
            Console.WriteLine($"Battery Power: {CurrentHistoryRow.BatteryCurrentPowerKw:F2} kW");
            Console.WriteLine($"Battery SoC: {CurrentHistoryRow.BatteryStateOfChargeKwh:F2} kWh");
            
            //extra info for battery state
            Console.WriteLine($"Battery State: {Neighbourhood.Battery?.State ?? "Idle"}");
            
            Console.WriteLine($"Total Energy: {CurrentHistoryRow.TotalEnergyKwh:F2} kWh");
            
            Console.WriteLine($"Peak Load (Without Battery): {CurrentHistoryRow.PeakWithoutBatteryKwh:F2} kW");
            Console.WriteLine($"Peak Load (With Battery): {CurrentHistoryRow.PeakWithBatteryKwh:F2} kW");

        }

        Clock.Tick();
    }
}

