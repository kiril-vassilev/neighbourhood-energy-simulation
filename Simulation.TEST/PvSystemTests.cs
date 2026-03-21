using Simulation.BLL.Domain;
using Simulation.BLL.Core;
using Xunit;


namespace Simulation.TEST;


public class PvSystemTests
{
    [Fact]
    public void Pv_ShouldGeneratePowerDuringDay()
    {
        var pv = new PvSystem();

        var ctx = new SimulationContext
        {
            Time = new DateTime(2024, 6, 1, 12, 0, 0),
            StepHours = 0.25,
            Weather = new Weather { SolarFactor = 1 }
        };

        pv.Update(ctx);

        Assert.True(pv.CurrentPowerKw < 0); // generation = negative
    }

    [Fact]
    public void Pv_ShouldGenerateZeroAtNight()
    {
        var pv = new PvSystem();

        var ctx = new SimulationContext
        {
            Time = new DateTime(2024, 6, 1, 2, 0, 0),
            StepHours = 0.25,
            Weather = new Weather { SolarFactor = 1 }
        };

        pv.Update(ctx);

        Assert.Equal(0, pv.CurrentPowerKw);
    }
}