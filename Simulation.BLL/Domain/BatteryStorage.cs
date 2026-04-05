using Simulation.BLL.Core;

namespace Simulation.BLL.Domain;

public class BatteryStorage : IEnergyAsset
{
    public double CapacityKWh { get; set; } = 100;
    public double StateOfChargeKWh { get; set; } = 50;

    public double MaxChargeKw { get; set; } = 20;
    public double MaxDischargeKw { get; set; } = 20;
    public double TargetLoadKw { get; set; } = 50;
    public double Efficiency { get; set; } = 0.95;
    public double DeadBandKw { get; set; } = 10;

    public double CurrentPowerKw { get; private set; }

    public double TotalEnergyKWh { get; private set; }

    public string State { get; private set; } = "Idle";

    public void Update(SimulationContext ctx, double currentNeighbourhoodLoadKw)
    {
        double desiredPower = 0;
        

        if (currentNeighbourhoodLoadKw > TargetLoadKw + DeadBandKw)
        {
            State = "Discharging";
            desiredPower = -(currentNeighbourhoodLoadKw - TargetLoadKw);
        }
        else if (currentNeighbourhoodLoadKw < TargetLoadKw - DeadBandKw)
        {
            State = "Charging";
            desiredPower = (TargetLoadKw - currentNeighbourhoodLoadKw);
        }
        else
        {
            State = "Idle";
            desiredPower = 0;
        }

        // Clamp to limits
        desiredPower = Math.Clamp(desiredPower, -MaxDischargeKw, MaxChargeKw);

        // Apply SoC constraints
        if (desiredPower < 0) 
        {
            // discharging
            double maxPossible = (StateOfChargeKWh * Efficiency) / ctx.StepHours;
            desiredPower = Math.Max(desiredPower, -maxPossible);
            if (desiredPower == -maxPossible) State = "Discharging (Empty)";
        }
        else 
        {
            // charging
            double remainingCapacity = CapacityKWh - StateOfChargeKWh;
            double maxPossible = (remainingCapacity / Efficiency) / ctx.StepHours;
            desiredPower = Math.Min(desiredPower, maxPossible);
            if (desiredPower == maxPossible) State = "Charging (Full)";
        }

        CurrentPowerKw = desiredPower;

        // Update SoC and track energy throughput 
        if (CurrentPowerKw < 0)
        {
            // Discharging: reduce SoC by actual energy delivered (accounting for efficiency)
            StateOfChargeKWh += CurrentPowerKw * ctx.StepHours / Efficiency;
        }
        else
        {
            // Charging: increase SoC by actual energy stored (accounting for efficiency)
            StateOfChargeKWh += CurrentPowerKw * ctx.StepHours * Efficiency;
        }

        // Track energy throughput
        TotalEnergyKWh += Math.Abs(CurrentPowerKw) * ctx.StepHours;
    }

    // IEnergyAsset compatibility (not used directly)
    public void Update(SimulationContext ctx)
    {
        // Not used
    }
}