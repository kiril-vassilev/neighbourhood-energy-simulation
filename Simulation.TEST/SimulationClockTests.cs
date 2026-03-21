using Simulation.BLL.Core;
using Xunit;

namespace Simulation.TEST;


public class SimulationClockTests
{
    [Fact]
    public void Tick_ShouldAdvanceTimeByStep()
    {
        var clock = new SimulationClock();
        var initial = clock.CurrentTime;

        clock.Tick();

        Assert.Equal(initial.AddMinutes(15), clock.CurrentTime);
    }
}