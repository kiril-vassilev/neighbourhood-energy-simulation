// =============================
// Program.cs
// =============================
using Simulation.BLL.Core;
using Simulation.DAL;

var settings = SimulationSettingsLoader.LoadOrDefault();
var sim = SimulationFactory.Create(settings);

while (sim.Clock.HasRemainingTime)
{
    sim.Step(true);
    Thread.Sleep(settings.Runtime.ConsoleLoopSleepMs);
} 

