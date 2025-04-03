using LearningSemanticKernel.Models;
using Microsoft.AspNetCore.SignalR;

namespace LearningSemanticKernel.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(ChatMessageModel message)
    {
        message.ConnectionId = Context.ConnectionId;

        await SendMessageAsync(message);
    }

    private async Task SendMessageAsync(ChatMessageModel message)
    {
        await Clients.Client(message.ConnectionId)
            .SendAsync("ReceiveMessage", message.User, message.Message);
    }
}
