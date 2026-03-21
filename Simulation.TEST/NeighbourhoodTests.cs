using Simulation.BLL.Core;
using Xunit;

namespace Simulation.TEST;

public class NeighbourhoodTests
{
    [Fact]
    public void Simulation_ShouldProduceLoad()
    {
        var sim = SimulationFactory.Create();

        sim.Step(false);

        Assert.NotEqual(0, sim.Neighbourhood.CurrentLoadKw);
    }

    [Fact]
    public void History_ShouldBeCappedAt96Entries()
    {
        var sim = SimulationFactory.Create();

        for (int i = 0; i < 200; i++)
            sim.Step(false);

        Assert.True(sim.Neighbourhood.History.Count <= 96);
    }
}