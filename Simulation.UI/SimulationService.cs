using Simulation.BLL.Core;

public class SimulationService
{
    private readonly SimulationEngine _engine;
    private readonly Timer _timer;

    public SimulationEngine Engine => _engine;

    public event Action? OnUpdated;

    public SimulationService()
    {
        var settings = SimulationSettingsLoader.LoadOrDefault();
        _engine = SimulationFactory.Create(settings);

        Timer? timer = null;
        timer = new Timer(_ =>
        {
            if (!_engine.Clock.HasRemainingTime)
            {
                timer?.Change(Timeout.Infinite, Timeout.Infinite);
                return;
            }

            _engine.Step(true);
            OnUpdated?.Invoke();
        }, null, 0, settings.Runtime.UiTimerIntervalMs);

        _timer = timer;
    }
}