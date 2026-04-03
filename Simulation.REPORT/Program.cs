using Simulation.DAL;
using Simulation.REPORT;

var rows = HistoryRepository.GetAllHistory();
 HistorySummaryReport.WriteTo(Console.Out, rows);
 Console.WriteLine();
HistoryTableReport.WriteTo(Console.Out, rows, firstNRows: 100);