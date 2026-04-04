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

    [Fact]
    public void HasRemainingTime_ShouldBeFalse_AfterEndDateIsExceeded()
    {
        var start = new DateTime(2024, 1, 1, 0, 0, 0);
        var end = new DateTime(2024, 1, 1, 0, 30, 0);
        var clock = new SimulationClock(start, TimeSpan.FromMinutes(15), end);

        Assert.True(clock.HasRemainingTime);

        clock.Tick();
        Assert.True(clock.HasRemainingTime);

        clock.Tick();
        Assert.True(clock.HasRemainingTime);

        clock.Tick();
        Assert.False(clock.HasRemainingTime);
    }

    [Fact]
    public void HasRemainingTime_ShouldStayTrue_WhenEndDateIsNull()
    {
        var clock = new SimulationClock(new DateTime(2024, 1, 1), TimeSpan.FromMinutes(15), null);

        for (int i = 0; i < 1000; i++)
            clock.Tick();

        Assert.True(clock.HasRemainingTime);
    }
}