// =============================
// Core/SimulationFactory.cs
// =============================
using Simulation.BLL.Domain;

namespace Simulation.BLL.Core;

public static class SimulationFactory
{
    public static SimulationEngine Create()
    {
        var rand = new Random(42);
        var neighbourhood = new Neighbourhood();

        neighbourhood.Battery = new BatteryStorage
        {
            CapacityKWh = 200,
            StateOfChargeKWh = 100,
            MaxChargeKw = 30,
            MaxDischargeKw = 30,
            TargetLoadKw = 60
        };

        for (int i = 0; i < 30; i++)
        {
            var house = new House();

            house.Assets.Add(new BaseLoad());

            if (rand.NextDouble() < 0.4)
                house.Assets.Add(new PvSystem());

            if (rand.NextDouble() < 0.3)
                house.Assets.Add(new HeatPump());

            if (rand.NextDouble() < 0.2)
                house.Assets.Add(new EvCharger());

            neighbourhood.Houses.Add(house);
        }

        for (int i = 0; i < 6; i++)
        {
            neighbourhood.PublicChargers.Add(new PublicCharger());
        }

        return new SimulationEngine(new SimulationClock(), neighbourhood);
    }
}
