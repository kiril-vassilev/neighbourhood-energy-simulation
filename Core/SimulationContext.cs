// =============================
// Core/SimulationContext.cs
// =============================
using Simulation.Domain;

namespace Simulation.Core;

public class SimulationContext
{
    public DateTime Time { get; set; }
    public double StepHours { get; set; }
    public Weather Weather { get; set; } = default!;
}
