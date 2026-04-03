// =============================
// Program.cs
// =============================
using Simulation.BLL.Core;
using Simulation.DAL;

var normalizedArgs = args
    .Where(a => !string.IsNullOrWhiteSpace(a))
    .Select(a => a.Trim())
    .ToArray();

var showHelp = normalizedArgs.Any(a => string.Equals(a, "--help", StringComparison.OrdinalIgnoreCase));
if (showHelp)
{
    PrintHelp();
    return;
}

var settings = SimulationSettingsLoader.LoadOrDefault();
var generateDataMode = normalizedArgs.Any(IsGenerateDataArg);

if (generateDataMode && !settings.Database.Enabled)
{
    Console.Error.WriteLine("generate_data mode requires database.enabled to be true.");
    Environment.Exit(1);
}

var sim = SimulationFactory.Create(settings);
var startTime = sim.Clock.CurrentTime;

if (generateDataMode && !sim.Clock.EndTime.HasValue)
{
    Console.Error.WriteLine("generate_data mode requires Simulation.EndTime to be configured.");
    Environment.Exit(1);
}

if (generateDataMode)
{
    Console.Write("Progress: 0%");
}

while (sim.Clock.HasRemainingTime)
{
    if (generateDataMode)
    {
        sim.Step(false);

        var progressPercent = CalculateProgressPercent(startTime, sim.Clock.CurrentTime, sim.Clock.EndTime!.Value);
        Console.Write($"\rProgress: {progressPercent}%");
    }
    else
    {
        sim.Step(true);
        Thread.Sleep(settings.Runtime.ConsoleLoopSleepMs);
    }
}

if (generateDataMode)
{
    Console.Write("\rProgress: 100%\n");
}

static int CalculateProgressPercent(DateTime startTime, DateTime currentTime, DateTime endTime)
{
    var totalSeconds = (endTime - startTime).TotalSeconds;
    if (totalSeconds <= 0)
        return 100;

    var elapsedSeconds = (currentTime - startTime).TotalSeconds;
    var ratio = elapsedSeconds / totalSeconds;
    var boundedRatio = Math.Clamp(ratio, 0d, 1d);

    return (int)Math.Round(boundedRatio * 100d, MidpointRounding.AwayFromZero);
}

static bool IsGenerateDataArg(string arg)
{
    return string.Equals(arg, "generate_data", StringComparison.OrdinalIgnoreCase)
        || string.Equals(arg, "generate-data", StringComparison.OrdinalIgnoreCase)
        || string.Equals(arg, "--generate-data", StringComparison.OrdinalIgnoreCase);
}

static void PrintHelp()
{
    Console.WriteLine("Usage: Simulation.BLL [options]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  generate_data      Run data generation mode (no sleep, progress output).");
    Console.WriteLine("  generate-data      Alias for generate_data.");
    Console.WriteLine("  --generate-data    Alias for generate_data.");
    Console.WriteLine("  --help             Show this help message.");
}

