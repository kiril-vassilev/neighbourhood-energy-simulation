using Simulation.BLL.Core;

public class SimulationService
{
    private readonly SimulationEngine _engine;
    private readonly Timer _timer;

    public SimulationEngine Engine => _engine;

    public event Action? OnUpdated;

    public SimulationService()
    {
        _engine = SimulationFactory.Create();

        _timer = new Timer(_ =>
        {
            _engine.Step(true);
            OnUpdated?.Invoke();
        }, null, 0, 500); // speed control
    }
}