// =============================
// Domain/Assets/BaseLoad.cs
// =============================
using Simulation.Core;

namespace Simulation.Domain;

public class BaseLoad : IEnergyAsset
{
    public double CurrentPowerKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

    public void Update(SimulationContext ctx)
    {
        int hour = ctx.Time.Hour;

        CurrentPowerKw = hour switch
        {
            >= 18 and <= 22 => 2.5,
            >= 7 and <= 9 => 1.5,
            _ => 0.5
        };

        TotalEnergyKWh += CurrentPowerKw * ctx.StepHours;
    }
}