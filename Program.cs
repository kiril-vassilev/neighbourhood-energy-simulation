// =============================
// Program.cs
// =============================
using Simulation.Core;
using Simulation.Domain;

var sim = SimulationFactory.Create();

while (true)
{
    sim.Step();
    Thread.Sleep(200); // controls speed
}