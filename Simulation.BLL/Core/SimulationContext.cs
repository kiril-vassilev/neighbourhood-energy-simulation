using Simulation.BLL.Domain;

namespace Simulation.BLL.Core;

public class SimulationContext
{
    public DateTime Time { get; set; }
    public double StepHours { get; set; }
    public Weather Weather { get; set; } = default!;
}
