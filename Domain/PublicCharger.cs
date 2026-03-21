// =============================
// Domain/Assets/PublicCharger.cs
// =============================
using Simulation.Core;

namespace Simulation.Domain;

public class PublicCharger : IEnergyAsset
{
    private Random _rand = new();

    public double CurrentPowerKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

    public void Update(SimulationContext ctx)
    {
        CurrentPowerKw = _rand.NextDouble() > 0.7 ? 22 : 0;
        TotalEnergyKWh += CurrentPowerKw * ctx.StepHours;
    }
}