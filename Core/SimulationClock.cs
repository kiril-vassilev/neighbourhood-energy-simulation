// =============================
// Core/SimulationClock.cs
// =============================
namespace Simulation.Core;

public class SimulationClock
{
    public DateTime CurrentTime { get; private set; } = new DateTime(2024, 1, 1);
    public TimeSpan StepSize { get; } = TimeSpan.FromMinutes(15);

    public void Tick()
    {
        CurrentTime = CurrentTime.Add(StepSize);
    }
}