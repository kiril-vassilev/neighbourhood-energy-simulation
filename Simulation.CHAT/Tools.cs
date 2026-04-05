using Simulation.BLL.Core;
using Simulation.DAL;
using Simulation.REPORT;

namespace  Simulation.CHAT;

public sealed class Tools   
{
    
    private SimulationSettingsRoot _settings;
    private IReadOnlyList<HistoryRow> _historyRows;

    public Tools(SimulationSettingsRoot settings)
    {
        _settings = settings;
        _historyRows =  HistoryRepository.GetAllHistory();
    }   


    public Task<string> GetHistorySummaryReportJsonAsync()
    {
        return Task.FromResult(HistorySummaryReport.GetJson(_historyRows, _settings.Simulation.StepMinutes, _settings.Battery.CapacityKWh));
    }   

    public Task<string> GetHistorySummaryPerSeasonReportJsonAsync()
    {
        return Task.FromResult(HistorySummaryPerSeasonReport.GetJson(_historyRows, _settings.Simulation.StepMinutes, _settings.Battery.CapacityKWh));
    }   

}