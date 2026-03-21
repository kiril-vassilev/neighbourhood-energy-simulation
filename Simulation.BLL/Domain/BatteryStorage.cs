// =============================
// Domain/BatteryStorage.cs
// =============================

using Simulation.BLL.Core;

namespace Simulation.BLL.Domain;

public class BatteryStorage : IEnergyAsset
{
    public double CapacityKWh { get; set; } = 100;
    public double StateOfChargeKWh { get; set; } = 50;

    public double MaxChargeKw { get; set; } = 20;
    public double MaxDischargeKw { get; set; } = 20;

    public double Efficiency { get; set; } = 0.95;

    public double CurrentPowerKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

    public double TargetLoadKw { get; set; } = 50;

    public void Update(SimulationContext ctx, double currentNeighbourhoodLoadKw)
    {
        double desiredPower = 0;

        if (currentNeighbourhoodLoadKw > TargetLoadKw)
        {
            // DISCHARGE (reduce peak)
            desiredPower = -(currentNeighbourhoodLoadKw - TargetLoadKw);
        }
        else if (currentNeighbourhoodLoadKw < TargetLoadKw)
        {
            // CHARGE (store excess / low load)
            desiredPower = (TargetLoadKw - currentNeighbourhoodLoadKw);
        }

        // Clamp to limits
        desiredPower = Math.Clamp(desiredPower, -MaxDischargeKw, MaxChargeKw);

        // Apply SoC constraints
        if (desiredPower < 0) // discharging
        {
            double maxPossible = StateOfChargeKWh / ctx.StepHours;
            desiredPower = Math.Max(desiredPower, -maxPossible);
        }
        else // charging
        {
            double remainingCapacity = CapacityKWh - StateOfChargeKWh;
            double maxPossible = remainingCapacity / ctx.StepHours;
            desiredPower = Math.Min(desiredPower, maxPossible);
        }

        CurrentPowerKw = desiredPower;

        // Update SoC
        StateOfChargeKWh += CurrentPowerKw * ctx.StepHours * Efficiency;

        // Track energy throughput
        TotalEnergyKWh += Math.Abs(CurrentPowerKw) * ctx.StepHours;
    }

    // IEnergyAsset compatibility (not used directly)
    public void Update(SimulationContext ctx)
    {
        // Not used
    }
}