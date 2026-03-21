// =============================
// Domain/Assets/PvSystem.cs
// =============================
using Simulation.Core;

namespace Simulation.Domain;

public class PvSystem : IEnergyAsset
{
    public double PeakKw { get; set; } = 4;

    public double CurrentPowerKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

    public void Update(SimulationContext ctx)
    {
        double hourFactor = Math.Max(0, Math.Sin((ctx.Time.Hour - 6) / 12.0 * Math.PI));
        CurrentPowerKw = -PeakKw * hourFactor * ctx.Weather.SolarFactor; // negative = generation

        TotalEnergyKWh += Math.Abs(CurrentPowerKw) * ctx.StepHours;
    }
}

