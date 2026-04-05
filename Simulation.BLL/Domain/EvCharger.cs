using Simulation.BLL.Core;

namespace Simulation.BLL.Domain;

public class EvCharger : IEnergyAsset
{
    private readonly int _fromHourInclusive;
    private readonly int _toHourInclusive;
    private readonly double _powerKw;

    public EvCharger(int fromHourInclusive = 18, int toHourInclusive = 23, double powerKw = 7)
    {
        _fromHourInclusive = fromHourInclusive;
        _toHourInclusive = toHourInclusive;
        _powerKw = powerKw;
    }

    public double CurrentPowerKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

    public void Update(SimulationContext ctx)
    {
        CurrentPowerKw = (ctx.Time.Hour >= _fromHourInclusive && ctx.Time.Hour <= _toHourInclusive)
            ? _powerKw
            : 0;
        TotalEnergyKWh += CurrentPowerKw * ctx.StepHours;
    }
}
