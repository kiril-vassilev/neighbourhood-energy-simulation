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

        if (DatabaseEnabled)
        {
            try
            {
                HistoryRepository.Insert(context.Time, Neighbourhood.CurrentLoadWithBatteryKw);
            }
            catch
            {
                // Database persistence is best-effort and should never stop the simulation loop.
            }
        }

        if (verbose) 
        {
            Console.Clear();
            Console.WriteLine($"Time: {Clock.CurrentTime}");
            Console.WriteLine($"Season: {Clock.Season}");
            Console.WriteLine($"Temp: {context.Weather.Temperature:F1}C");
            Console.WriteLine($"Load: {Neighbourhood.CurrentLoadKw:F2} kW");

            Console.WriteLine($"Load (With Battery): {Neighbourhood.CurrentLoadWithBatteryKw:F2} kW");
            Console.WriteLine($"Battery Power: {Neighbourhood.Battery?.CurrentPowerKw:F2} kW");
            Console.WriteLine($"Battery SoC: {Neighbourhood.Battery?.StateOfChargeKWh:F2} kWh");
            Console.WriteLine($"Total Energy: {Neighbourhood.TotalEnergyKWh:F2} kWh");
            
            Console.WriteLine($"Peak Load (Without Battery): {Neighbourhood.PeakWithoutBattery:F2} kW");
            Console.WriteLine($"Peak Load (With Battery): {Neighbourhood.PeakWithBattery:F2} kW");

        }

        Clock.Tick();
    }
}

