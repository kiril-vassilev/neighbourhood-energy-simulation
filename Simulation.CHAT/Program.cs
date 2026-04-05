using Simulation.BLL.Core;
using Simulation.DAL;
using Simulation.REPORT;

var settings = SimulationSettingsLoader.LoadOrDefault();
int stepMinutes = settings.Simulation.StepMinutes;
double batteryCapacityKwh = settings.Battery.CapacityKWh;

var rows = HistoryRepository.GetAllHistory();

Console.WriteLine("Neighborhood Energy Simulation Chatbot");
Console.WriteLine("Type your question and press Enter. Type 'quit' to exit.");
Console.WriteLine(new string('-', 50));

while (true)
{
    Console.Write("You: ");
    string? input = Console.ReadLine();

    if (input == null || input.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase))
        break;

    if (string.IsNullOrWhiteSpace(input))
        continue;

    Console.WriteLine($"Bot: {input}");
    Console.WriteLine();
}

Console.WriteLine("Goodbye!");
