// =============================
// Domain/Assets/HeatPump.cs
// =============================
using Simulation.Core;

namespace Simulation.Domain;

public class HeatPump : IEnergyAsset
{
    public double CurrentPowerKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

    public void Update(SimulationContext ctx)
    {
        var temp = ctx.Weather.Temperature;

        CurrentPowerKw = temp > 18 ? 0 : (18 - temp) * 0.3;
        TotalEnergyKWh += CurrentPowerKw * ctx.StepHours;
    }
}