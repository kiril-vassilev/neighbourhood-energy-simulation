// =============================
// Domain/Neighbourhood.cs
// =============================
using Simulation.BLL.Core;

namespace Simulation.BLL.Domain;

public class Neighbourhood
{
    public List<House> Houses { get; } = new();
    public List<PublicCharger> PublicChargers { get; } = new();

    public double CurrentLoadKw { get; private set; }
    public double TotalEnergyKWh { get; private set; }

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
        TotalEnergyKWh += total * context.StepHours;

        History.Add((context.Time, total));

        if (History.Count > 96)
            History.RemoveAt(0);
    }
}