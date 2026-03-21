// =============================
// Program.cs
// =============================
using Simulation.BLL.Core;
using Simulation.BLL.Domain;

var sim = SimulationFactory.Create();

while (true)
{
    sim.Step();
    Thread.Sleep(200); // controls speed
}