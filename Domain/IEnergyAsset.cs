// =============================
// Domain/Interfaces/IEnergyAsset.cs
// =============================
using Simulation.Core;

namespace Simulation.Domain;

public interface IEnergyAsset
{
    double CurrentPowerKw { get; }
    double TotalEnergyKWh { get; }

    void Update(SimulationContext context);
}
