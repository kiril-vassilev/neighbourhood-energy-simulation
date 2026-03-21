using Simulation.BLL.Domain;
using Simulation.BLL.Core;
using Xunit;


namespace Simulation.TEST;


public class EnergyCalculationTests
{
    [Fact]
    public void Asset_ShouldAccumulateEnergyCorrectly()
    {
        var asset = new BaseLoad();

        var ctx = new SimulationContext
        {
            Time = new DateTime(2024, 1, 1, 20, 0, 0), // peak time
            StepHours = 0.25, // 15 min
            Weather = new Weather()
        };

        asset.Update(ctx);

        // 2.5 kW * 0.25h = 0.625 kWh
        Assert.Equal(0.625, asset.TotalEnergyKWh, 3);
    }
}