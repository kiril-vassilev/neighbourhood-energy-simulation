// =============================
// Domain/Interfaces/IEnergyAsset.cs
// =============================
using Simulation.BLL.Core;

namespace Simulation.BLL.Domain;

public interface IEnergyAsset
{
    double CurrentPowerKw { get; }
    double TotalEnergyKWh { get; }

    void Update(SimulationContext context);
}
