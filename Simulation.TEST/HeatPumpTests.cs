using Simulation.BLL.Domain;
using Simulation.BLL.Core;
using Xunit;

namespace Simulation.TEST;


public class HeatPumpTests
{
    [Fact]
    public void HeatPump_ShouldConsumeMore_WhenCold()
    {
        var hp = new HeatPump();

        var coldCtx = new SimulationContext
        {
            Time = DateTime.Now,
            StepHours = 0.25,
            Weather = new Weather { Temperature = 0 }
        };

        var warmCtx = new SimulationContext
        {
            Time = DateTime.Now,
            StepHours = 0.25,
            Weather = new Weather { Temperature = 20 }
        };

        hp.Update(coldCtx);
        var coldPower = hp.CurrentPowerKw;

        hp.Update(warmCtx);
        var warmPower = hp.CurrentPowerKw;

        Assert.True(coldPower > warmPower);
    }
}
