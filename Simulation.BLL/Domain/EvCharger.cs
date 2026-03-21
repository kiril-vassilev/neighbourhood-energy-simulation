// =============================
// Domain/Assets/EvCharger.cs
// =============================
using Simulation.BLL.Core;

namespace Simulation.BLL.Domain;

public class EvCharger : IEnergyAsset
{
    public double CurrentPowerKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

    public void Update(SimulationContext ctx)
    {
        CurrentPowerKw = (ctx.Time.Hour >= 18 && ctx.Time.Hour <= 23) ? 7 : 0;
        TotalEnergyKWh += CurrentPowerKw * ctx.StepHours;
    }
}
