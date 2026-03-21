// =============================
// Core/SimulationEngine.cs
// =============================

using Simulation.BLL.Domain;

namespace Simulation.BLL.Core;

public class SimulationEngine
{
    public SimulationClock Clock { get; }
    public Neighbourhood Neighbourhood { get; }

    public SimulationEngine(SimulationClock clock, Neighbourhood neighbourhood)
    {
        Clock = clock;
        Neighbourhood = neighbourhood;
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

        if (!verbose) return;
        
        Console.Clear();
        Console.WriteLine($"Time: {Clock.CurrentTime}");
        Console.WriteLine($"Temp: {context.Weather.Temperature:F1}C");
        Console.WriteLine($"Load: {Neighbourhood.CurrentLoadKw:F2} kW");
        Console.WriteLine($"Total Energy: {Neighbourhood.TotalEnergyKWh:F2} kWh");

        Clock.Tick();
    }
}

