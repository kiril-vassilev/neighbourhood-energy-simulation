using Simulation.BLL.Core;

namespace Simulation.BLL.Domain;

public class House
{
    public List<IEnergyAsset> Assets { get; } = new();

    public double Update(SimulationContext ctx)
    {
        double total = 0;

        foreach (var asset in Assets)
        {
            asset.Update(ctx);
            total += asset.CurrentPowerKw;
        }

        return total;
    }
}

