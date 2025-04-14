using LearningSemanticKernel.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LearningSemanticKernel.Hubs;

public class ChatHub : Hub
{
    private readonly IChatCompletionService _chatCompletionService;

    public ChatHub(IChatCompletionService chatCompletionService)
    {
        _chatCompletionService = chatCompletionService;
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

        try
        {
            var res = _chatCompletionService.GetStreamingChatMessageContentsAsync(message.Message);

            await StreamAsync(message.ConnectionId, res);
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
