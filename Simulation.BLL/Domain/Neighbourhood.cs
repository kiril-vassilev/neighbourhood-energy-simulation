// =============================
// Domain/Neighbourhood.cs
// =============================
using Simulation.BLL.Core;

namespace Simulation.BLL.Domain;

public class Neighbourhood
{
    public List<House> Houses { get; } = new();
    public List<PublicCharger> PublicChargers { get; } = new();
    public int HistoryCapacity { get; set; } = 96;

    public double CurrentLoadKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

    public BatteryStorage? Battery { get; set; }
    public double CurrentLoadWithBatteryKw { get; private set; }

    public double PeakWithoutBattery { get; private set; }
    public double PeakWithBattery { get; private set; }
    
    public List<(DateTime time, double load)> History { get; } = new();

    public void Update(SimulationContext context)
    {
        double total = 0;

        foreach (var house in Houses)
            total += house.Update(context);

        foreach (var charger in PublicChargers)
        {
            charger.Update(context);
            total += charger.CurrentPowerKw;
        }

        CurrentLoadKw = total;

        // Battery interaction
        double batteryPower = 0;

        if (Battery != null)
        {
            Battery.Update(context, total);
            batteryPower = Battery.CurrentPowerKw;
        }

        CurrentLoadWithBatteryKw = total + batteryPower;

        TotalEnergyKWh += CurrentLoadWithBatteryKw * context.StepHours;

        PeakWithoutBattery = Math.Max(PeakWithoutBattery, CurrentLoadKw);
        PeakWithBattery = Math.Max(PeakWithBattery, CurrentLoadWithBatteryKw);

        History.Add((context.Time, CurrentLoadWithBatteryKw));

        if (History.Count > HistoryCapacity)
            History.RemoveAt(0);
    }
}