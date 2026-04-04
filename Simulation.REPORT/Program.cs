using Simulation.BLL.Core;
using Simulation.DAL;
using Simulation.REPORT;

var settings = SimulationSettingsLoader.LoadOrDefault();
int stepMinutes = settings.Simulation.StepMinutes;
double batteryCapacityKwh = settings.Battery.CapacityKWh;

var rows = HistoryRepository.GetAllHistory();

// Console.WriteLine(HistorySummaryReport.GetJson(rows, stepMinutes, batteryCapacityKwh));
// Console.WriteLine();
// Console.WriteLine(HistorySummaryPerSeasonReport.GetJson(rows, stepMinutes, batteryCapacityKwh));

HistorySummaryReport.WriteTo(Console.Out, rows, stepMinutes, batteryCapacityKwh);
Console.WriteLine();
HistorySummaryPerSeasonReport.WriteTo(Console.Out, rows, stepMinutes, batteryCapacityKwh);
Console.WriteLine();
HistoryTableReport.WriteTo(Console.Out, rows, firstNRows: 100);