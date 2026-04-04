// =============================
// Domain/Assets/BaseLoad.cs
// =============================
using Simulation.BLL.Core;

namespace Simulation.BLL.Domain;

public class BaseLoad : IEnergyAsset
{
    public readonly record struct Rule(int FromHourInclusive, int ToHourInclusive, double PowerKw);

    private readonly List<Rule> _rules;
    private readonly double _defaultPowerKw;

    public BaseLoad(IEnumerable<Rule>? rules = null, double defaultPowerKw = 0.5)
    {
        _rules = rules?.ToList() ??
        [
            new Rule(18, 22, 2.5),
            new Rule(7, 9, 1.5)
        ];
        _defaultPowerKw = defaultPowerKw;
    }

    public double CurrentPowerKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

    public void Update(SimulationContext ctx)
    {
        int hour = ctx.Time.Hour;

        var matchingRule = _rules.FirstOrDefault(r => hour >= r.FromHourInclusive && hour <= r.ToHourInclusive);
        CurrentPowerKw = matchingRule == default ? _defaultPowerKw : matchingRule.PowerKw;

        TotalEnergyKWh += CurrentPowerKw * ctx.StepHours;
    }
}