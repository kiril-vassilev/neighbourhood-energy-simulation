// =============================
// Program.cs
// =============================
using Simulation.BLL.Core;
using Simulation.BLL.Domain;

var settings = SimulationSettingsLoader.LoadOrDefault();
var sim = SimulationFactory.Create(settings);

while (sim.Clock.HasRemainingTime)
{
    sim.Step(true);
    Thread.Sleep(settings.Runtime.ConsoleLoopSleepMs);
} 