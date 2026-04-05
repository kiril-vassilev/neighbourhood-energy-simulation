using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

using Simulation.BLL.Core;

namespace Simulation.CHAT;

public class Chatbot
{
    
    public AIAgent? Agent { get; private set; }
    public AgentSession? Session { get; private set; } 

    private SimulationSettingsRoot? _settings;
    private Tools? _tools;

    private string GetInstructions() =>
        "You are a helpful assistant that answers questions about the neighborhood energy simulation. " +
        "Use the following information to answer questions:\n\n" +
        $"Simulation starts at: {_settings?.Simulation.StartTime}\n" +
        $"Simulation step minutes: {_settings?.Simulation.StepMinutes}\n" +
        $"Battery capacity (kWh): {_settings?.Battery.CapacityKWh}\n" +
        "Use GetHistorySummaryReportJson to get a JSON report summarizing the simulation history.\n" +
        "You can also use GetHistorySummaryPerSeasonReportJson to get a JSON report summarizing the simulation history per season.";


    public async Task InitializeAsync(SimulationSettingsRoot settings)
    {
        _settings = settings;
        _tools = new Tools(_settings);

        string endpoint = settings.Chatbot.AzureOpenAI_Endpoint;
        string deploymentName = settings.Chatbot.AzureOpenAI_DeploymentName;    
    
        Agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new DefaultAzureCredential())
            .GetChatClient(deploymentName)
            .AsAIAgent(
                instructions: GetInstructions(), 
                name: "SimulationAssistant",
                tools: [
                    AIFunctionFactory.Create(_tools.GetHistorySummaryReportJsonAsync, "GetHistorySummaryReportJson", "Gets a JSON report summarizing the simulation history."),
                    AIFunctionFactory.Create(_tools.GetHistorySummaryPerSeasonReportJsonAsync, "GetHistorySummaryPerSeasonReportJson", "Gets a JSON report summarizing the simulation history per season.")
                ]
            );

        Session = await Agent.CreateSessionAsync();
    }

    public async Task<string> AskQuestionAsync(string question)
    {
        if (Agent == null || Session == null)
            throw new InvalidOperationException("Chatbot is not initialized.");

        var response = await Agent.RunAsync(question, Session);

        return response.Messages.LastOrDefault()?.Text ?? "Sorry, I couldn't generate a response.";
    }



}

