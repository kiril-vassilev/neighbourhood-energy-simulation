// =============================
// Domain/Assets/HeatPump.cs
// =============================
using Simulation.BLL.Core;

namespace Simulation.BLL.Domain;

public class HeatPump : IEnergyAsset
{
    private readonly double _offAboveTemperatureC;
    private readonly double _powerKwPerDegreeBelowThreshold;

    public HeatPump(double offAboveTemperatureC = 18, double powerKwPerDegreeBelowThreshold = 0.3)
    {
        _offAboveTemperatureC = offAboveTemperatureC;
        _powerKwPerDegreeBelowThreshold = powerKwPerDegreeBelowThreshold;
    }

    public double CurrentPowerKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

    public void Update(SimulationContext ctx)
    {
        var temp = ctx.Weather.Temperature;

        CurrentPowerKw = temp > _offAboveTemperatureC
            ? 0
            : (_offAboveTemperatureC - temp) * _powerKwPerDegreeBelowThreshold;
        TotalEnergyKWh += CurrentPowerKw * ctx.StepHours;
    }
}