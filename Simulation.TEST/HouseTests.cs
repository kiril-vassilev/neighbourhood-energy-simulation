using Simulation.BLL.Domain;
using Simulation.BLL.Core;
using Xunit;

namespace Simulation.TEST;

public class HouseTests
{
    [Fact]
    public void House_ShouldAggregateAllAssets()
    {
        var house = new House();

        house.Assets.Add(new BaseLoad());
        house.Assets.Add(new EvCharger());

        var ctx = new SimulationContext
        {
            Time = new DateTime(2024, 1, 1, 19, 0, 0),
            StepHours = 0.25,
            Weather = new Weather()
        };

        var total = house.Update(ctx);

        Assert.True(total > 0);
    }
}
