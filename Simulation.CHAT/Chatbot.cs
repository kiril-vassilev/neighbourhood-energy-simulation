using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI.Chat;

using Simulation.BLL.Core;

namespace Simulation.CHAT;

public class Chatbot
{
    
    public AIAgent? Agent { get; private set; }
    public AgentSession? Session { get; private set; } 

    private SimulationSettingsRoot? _settings;


    public async Task InitializeAsync(SimulationSettingsRoot settings)
    {
        _settings = settings;

        string endpoint = settings.Chatbot.AzureOpenAI_Endpoint;
        string deploymentName = settings.Chatbot.AzureOpenAI_DeploymentName;    
    
        Agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new DefaultAzureCredential())
            .GetChatClient(deploymentName)
            .AsAIAgent(instructions: "You are good at telling jokes.", name: "Joker");

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

