using Simulation.BLL.Core;
using Simulation.DAL;
using Simulation.REPORT;

if (args.Length == 0 || args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
{
    PrintHelp();
    return;
}

var settings = SimulationSettingsLoader.LoadOrDefault();
int stepMinutes = settings.Simulation.StepMinutes;
double batteryCapacityKwh = settings.Battery.CapacityKWh;

var rows = HistoryRepository.GetAllHistory();

switch (args[0])
{
    case "1":
        HistorySummaryReport.WriteTo(Console.Out, rows, stepMinutes, batteryCapacityKwh);
        break;
    case "2":
        HistorySummaryPerSeasonReport.WriteTo(Console.Out, rows, stepMinutes, batteryCapacityKwh);
        break;
    case "3":
        HistoryTableReport.WriteTo(Console.Out, rows, firstNRows: 100);
        break;
    default:
        Console.Error.WriteLine($"Unknown parameter: '{args[0]}'");
        Console.Error.WriteLine();
        PrintHelp(Console.Error);
        Environment.Exit(1);
        break;
}

void PrintHelp(TextWriter? writer = null)
{
    writer ??= Console.Out;
    writer.WriteLine("Usage: Simulation.REPORT <parameter>");
    writer.WriteLine();
    writer.WriteLine("Parameters:");
    writer.WriteLine("  1      Print History Summary Report");
    writer.WriteLine("  2      Print History Summary Report per Season");
    writer.WriteLine("  3      Print History Report (first 100 rows)");
    writer.WriteLine("  help   Show this help message");
    writer.WriteLine();
    writer.WriteLine("Examples:");
    writer.WriteLine("  Simulation.REPORT 1");
    writer.WriteLine("  Simulation.REPORT 2");
    writer.WriteLine("  Simulation.REPORT 3");
    writer.WriteLine("  Simulation.REPORT help");
}