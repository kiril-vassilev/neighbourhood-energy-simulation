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
}