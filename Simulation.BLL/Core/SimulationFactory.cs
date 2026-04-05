// =============================
// Core/SimulationFactory.cs
// =============================
using Simulation.BLL.Domain;
using Simulation.DAL;

namespace Simulation.BLL.Core;

public static class SimulationFactory
{
    public static SimulationEngine Create(SimulationSettingsRoot? settings = null, bool isPresenting = true)
    {
        settings ??= SimulationSettingsLoader.LoadOrDefault();

        var rand = new Random(settings.Simulation.RandomSeed);

        WeatherGenerator.Configure(
            settings.Weather.SeasonBaseTemperatureC.Winter,
            settings.Weather.SeasonBaseTemperatureC.Spring,
            settings.Weather.SeasonBaseTemperatureC.Summer,
            settings.Weather.SeasonBaseTemperatureC.Fall);

        var baseLoadRules = settings.Assets.BaseLoad.PowerKwByHourRanges
            .Where(r => r.FromHourInclusive.HasValue && r.ToHourInclusive.HasValue && r.PowerKw.HasValue)
            .Select(r => new BaseLoad.Rule(r.FromHourInclusive!.Value, r.ToHourInclusive!.Value, r.PowerKw!.Value));

        var baseLoadDefault = settings.Assets.BaseLoad.PowerKwByHourRanges
            .FirstOrDefault(r => r.DefaultPowerKw.HasValue)
            ?.DefaultPowerKw ?? 0.5;

        // Clear history if not presenting (i.e., in generate-data mode) 
        if (!isPresenting)
            HistoryRepository.ClearHistory();

        var neighbourhood = new Neighbourhood
        {
            HistoryCapacity = settings.Simulation.HistoryPoints,

            Battery = new BatteryStorage
            {
                CapacityKWh = settings.Battery.CapacityKWh,
                StateOfChargeKWh = settings.Battery.StateOfChargeKWh,
                MaxChargeKw = settings.Battery.MaxChargeKw,
                MaxDischargeKw = settings.Battery.MaxDischargeKw,
                TargetLoadKw = settings.Battery.TargetLoadKw,
                Efficiency = settings.Battery.Efficiency,
                DeadBandKw = settings.Battery.DeadBandKw
            }
        };

        for (int i = 0; i < settings.Neighbourhood.Houses; i++)
        {
            var house = new House();

            house.Assets.Add(new BaseLoad(baseLoadRules, baseLoadDefault));

            if (rand.NextDouble() < settings.Neighbourhood.AssetDistributionProbability.PvSystem)
                house.Assets.Add(new PvSystem { PeakKw = settings.Assets.PvSystem.PeakKw });

            if (rand.NextDouble() < settings.Neighbourhood.AssetDistributionProbability.HeatPump)
                house.Assets.Add(new HeatPump(
                    settings.Assets.HeatPump.OffAboveTemperatureC,
                    settings.Assets.HeatPump.PowerKwPerDegreeBelowThreshold));

            if (rand.NextDouble() < settings.Neighbourhood.AssetDistributionProbability.HomeEvCharger)
                house.Assets.Add(new EvCharger(
                    settings.Assets.HomeEvCharger.ChargingWindow.FromHourInclusive,
                    settings.Assets.HomeEvCharger.ChargingWindow.ToHourInclusive,
                    settings.Assets.HomeEvCharger.PowerKw));

            neighbourhood.Houses.Add(house);
        }

        for (int i = 0; i < settings.Neighbourhood.PublicChargers; i++)
        {
            neighbourhood.PublicChargers.Add(new PublicCharger(
                rand,
                settings.Assets.PublicCharger.OccupiedIfRandomGreaterThan,
                settings.Assets.PublicCharger.OccupiedPowerKw,
                settings.Assets.PublicCharger.IdlePowerKw));
        }

        return new SimulationEngine(
            new SimulationClock(
                settings.Simulation.StartTime,
                TimeSpan.FromMinutes(settings.Simulation.StepMinutes),
                settings.Simulation.EndTime),
            neighbourhood);
    }
}
