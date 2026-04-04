// =============================
// Core/SimulationClock.cs
// =============================
namespace Simulation.BLL.Core;

public class SimulationClock
{
    public DateTime CurrentTime { get; private set; }
    public DateTime? EndTime { get; }

    public bool HasRemainingTime => !EndTime.HasValue || CurrentTime <= EndTime;

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

    public TimeSpan StepSize { get; }

    public SimulationClock()
        : this(new DateTime(2024, 1, 1), TimeSpan.FromMinutes(15), null)
    {
    }

    public SimulationClock(DateTime startTime, TimeSpan stepSize, DateTime? endTime = null)
    {
        CurrentTime = startTime;
        StepSize = stepSize;
        EndTime = endTime;
    }

    public void Tick()
    {
        if (!HasRemainingTime)
            return;

        CurrentTime = CurrentTime.Add(StepSize);
    }
}