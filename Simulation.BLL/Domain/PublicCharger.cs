using Simulation.BLL.Core;

namespace Simulation.BLL.Domain;

public class PublicCharger : IEnergyAsset
{
    private readonly Random _rand;
    private readonly double _occupiedIfRandomGreaterThan;
    private readonly double _occupiedPowerKw;
    private readonly double _idlePowerKw;

    public PublicCharger(
        Random? rand = null,
        double occupiedIfRandomGreaterThan = 0.7,
        double occupiedPowerKw = 22,
        double idlePowerKw = 0)
    {
        _rand = rand ?? new Random();
        _occupiedIfRandomGreaterThan = occupiedIfRandomGreaterThan;
        _occupiedPowerKw = occupiedPowerKw;
        _idlePowerKw = idlePowerKw;
    }

    public double CurrentPowerKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

    public void Update(SimulationContext ctx)
    {
        CurrentPowerKw = _rand.NextDouble() > _occupiedIfRandomGreaterThan
            ? _occupiedPowerKw
            : _idlePowerKw;
        TotalEnergyKWh += CurrentPowerKw * ctx.StepHours;
    }
}