using LearningSemanticKernel.Models;
using LearningSemanticKernel.Plugins;
using LearningSemanticKernel.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

#pragma warning disable SKEXP0070

namespace LearningSemanticKernel.Hubs;

public class ChatHub : Hub
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly IWeatherReportService _weatherReportService;

    public ChatHub(Kernel kernel,
        IChatCompletionService chatCompletionService,
        IWeatherReportService weatherReportService)
    {
        _kernel = kernel;
        _chatCompletionService = chatCompletionService;
        _weatherReportService = weatherReportService;
    }
    public async Task SendMessage(ChatMessageModel message)
    {
        message.ConnectionId = Context.ConnectionId;

        await SendMessageAsync(message);
    }

    private async Task SendMessageAsync(ChatMessageModel message)
    {
        await Clients.Client(message.ConnectionId)
            .SendAsync("ReceiveMessage", message.User, message.Message);

        _kernel.Plugins.AddFromObject(new WeatherReportPlugin(_weatherReportService));

        var executionSettings = new OllamaPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = 0.1f,
        };

        var history = new ChatHistory();
        history.AddUserMessage(message.Message);

        try
        {
            var res = await _chatCompletionService.GetChatMessageContentAsync(history, executionSettings, _kernel);
            await Clients.Client(message.ConnectionId)
                .SendAsync("ReceiveMessage", res.Role.ToString(), res.Content);
            // llama doesn't support function call in streaming
            //await StreamAsync(message.ConnectionId, res);
        }
        catch (Exception ex)
        {
            await Clients.Client(message.ConnectionId)
                .SendAsync("ReceiveMessage", "Error", ex.Message);
        }
    }

    private async Task StreamAsync(string connectionId, IAsyncEnumerable<StreamingChatMessageContent> res)
    {
        var first = true;
        await foreach (var content in res.ConfigureAwait(false))
        {
            if (content.Role.HasValue && first)
            {
                await Clients.Client(connectionId)
                    .SendAsync("ReceiveMessage", content.Role.ToString(), "");
                first = false;
            }

            await Clients.Client(connectionId)
                .SendAsync("ReceiveMessage", "", content.Content);
        }
    }
}
