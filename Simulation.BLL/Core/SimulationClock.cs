// =============================
// Core/SimulationClock.cs
// =============================
namespace Simulation.BLL.Core;

public class SimulationClock
{
    public DateTime CurrentTime { get; private set; } = new DateTime(2024, 1, 1);

    public string Season
    {
        get
        {
            int month = CurrentTime.Month;
            if (month >= 3 && month <= 5) return "Spring";
            if (month >= 6 && month <= 8) return "Summer";
            if (month >= 9 && month <= 11) return "Fall";
            return "Winter";
        }
    }

    public TimeSpan StepSize { get; } = TimeSpan.FromMinutes(15);

    public void Tick()
    {
        CurrentTime = CurrentTime.Add(StepSize);
    }
}