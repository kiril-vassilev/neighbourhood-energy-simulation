using System.Text.Json;

namespace Simulation.BLL.Core;

public class SimulationSettingsRoot
{
    public SimulationSettings Simulation { get; set; } = new();
    public NeighbourhoodSettings Neighbourhood { get; set; } = new();
    public AssetSettings Assets { get; set; } = new();
    public BatterySettings Battery { get; set; } = new();
    public WeatherSettings Weather { get; set; } = new();
    public RuntimeSettings Runtime { get; set; } = new();
    public DatabaseSettings Database { get; set; } = new();
}

public class SimulationSettings
{
    public DateTime StartTime { get; set; } = new DateTime(2024, 1, 1);
    public DateTime? EndTime { get; set; }
    public int StepMinutes { get; set; } = 15;
    public int RandomSeed { get; set; } = 42;
    public int HistoryPoints { get; set; } = 96;
}

public class NeighbourhoodSettings
{
    public int Houses { get; set; } = 30;
    public int PublicChargers { get; set; } = 6;
    public AssetDistributionSettings AssetDistributionProbability { get; set; } = new();
}

public class AssetDistributionSettings
{
    public double PvSystem { get; set; } = 0.4;
    public double HeatPump { get; set; } = 0.3;
    public double HomeEvCharger { get; set; } = 0.2;
}

public class AssetSettings
{
    public BaseLoadSettings BaseLoad { get; set; } = new();
    public PvSystemSettings PvSystem { get; set; } = new();
    public HeatPumpSettings HeatPump { get; set; } = new();
    public HomeEvChargerSettings HomeEvCharger { get; set; } = new();
    public PublicChargerSettings PublicCharger { get; set; } = new();
}

public class BaseLoadSettings
{
    public List<BaseLoadRangeSettings> PowerKwByHourRanges { get; set; } =
    [
        new BaseLoadRangeSettings { FromHourInclusive = 18, ToHourInclusive = 22, PowerKw = 2.5 },
        new BaseLoadRangeSettings { FromHourInclusive = 7, ToHourInclusive = 9, PowerKw = 1.5 },
        new BaseLoadRangeSettings { DefaultPowerKw = 0.5 }
    ];
}

public class BaseLoadRangeSettings
{
    public int? FromHourInclusive { get; set; }
    public int? ToHourInclusive { get; set; }
    public double? PowerKw { get; set; }
    public double? DefaultPowerKw { get; set; }
}

public class PvSystemSettings
{
    public double PeakKw { get; set; } = 4;
}

public class HeatPumpSettings
{
    public double OffAboveTemperatureC { get; set; } = 18;
    public double PowerKwPerDegreeBelowThreshold { get; set; } = 0.3;
}

public class HomeEvChargerSettings
{
    public ChargingWindowSettings ChargingWindow { get; set; } = new();
    public double PowerKw { get; set; } = 7;
}

public class ChargingWindowSettings
{
    public int FromHourInclusive { get; set; } = 18;
    public int ToHourInclusive { get; set; } = 23;
}

public class PublicChargerSettings
{
    public double OccupiedIfRandomGreaterThan { get; set; } = 0.7;
    public double OccupiedPowerKw { get; set; } = 22;
    public double IdlePowerKw { get; set; } = 0;
}

public class BatterySettings
{
    public double CapacityKWh { get; set; } = 200;
    public double StateOfChargeKWh { get; set; } = 100;
    public double MaxChargeKw { get; set; } = 30;
    public double MaxDischargeKw { get; set; } = 30;
    public double TargetLoadKw { get; set; } = 60;
    public double Efficiency { get; set; } = 0.95;
    public double DeadBandKw { get; set; } = 10;
}

public class WeatherSettings
{
    public SeasonBaseTemperatureSettings SeasonBaseTemperatureC { get; set; } = new();
}

public class SeasonBaseTemperatureSettings
{
    public double Winter { get; set; } = 5;
    public double Spring { get; set; } = 12;
    public double Summer { get; set; } = 22;
    public double Fall { get; set; } = 14;
}

public class RuntimeSettings
{
    public int ConsoleLoopSleepMs { get; set; } = 200;
    public int UiTimerIntervalMs { get; set; } = 500;
}

public class DatabaseSettings
{
    public bool Enabled { get; set; } = true;
    public bool ClearOnStart { get; set; } = true;
}

public static class SimulationSettingsLoader
{
    private const string SettingsFileName = "simulation.json";

    public static SimulationSettingsRoot LoadOrDefault()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        try
        {
            var configuredPath = Environment.GetEnvironmentVariable("SIMULATION_SETTINGS_PATH");
            if (!string.IsNullOrWhiteSpace(configuredPath) && File.Exists(configuredPath))
            {
                var configuredJson = File.ReadAllText(configuredPath);
                var configuredSettings = JsonSerializer.Deserialize<SimulationSettingsRoot>(configuredJson, options);
                if (configuredSettings != null)
                    return configuredSettings;
            }

            var fromCurrentDirectory = FindInCurrentDirectoryTree();
            if (fromCurrentDirectory != null)
            {
                var json = File.ReadAllText(fromCurrentDirectory);
                var settings = JsonSerializer.Deserialize<SimulationSettingsRoot>(json, options);
                if (settings != null)
                    return settings;
            }

            var fromAppBaseDirectory = FindInAppBaseDirectoryTree();
            if (fromAppBaseDirectory != null)
            {
                var json = File.ReadAllText(fromAppBaseDirectory);
                var settings = JsonSerializer.Deserialize<SimulationSettingsRoot>(json, options);
                if (settings != null)
                    return settings;
            }
        }
        catch
        {
            // Fall back to defaults if configuration cannot be loaded.
        }

        return new SimulationSettingsRoot();
    }

    private static string? FindInCurrentDirectoryTree()
    {
        return FindInDirectoryTree(Directory.GetCurrentDirectory());
    }

    private static string? FindInAppBaseDirectoryTree()
    {
        return FindInDirectoryTree(AppContext.BaseDirectory);
    }

    private static string? FindInDirectoryTree(string startDirectory)
    {
        try
        {
            var current = new DirectoryInfo(startDirectory);

            while (current != null)
            {
                var candidate = Path.Combine(current.FullName, SettingsFileName);
                if (File.Exists(candidate))
                    return candidate;

                current = current.Parent;
            }
        }
        catch
        {
            return null;
        }

        return null;
    }
}