using LearningSemanticKernel.Helpers;
using LearningSemanticKernel.Models;
using LearningSemanticKernel.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LearningSemanticKernel.Hubs;

public class ChatHub : Hub
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly IProductCatalogService _productCatalogService;

    public ChatHub(Kernel kernel,
        IChatCompletionService chatCompletionService,
        IProductCatalogService productCatalogService)
    {
        _kernel = kernel;
        _chatCompletionService = chatCompletionService;
        _productCatalogService = productCatalogService;
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

        var context = await _productCatalogService.FindRelatedContextDataAsync(message.Message);

        var plugin = _kernel.CreatePluginFromPromptDirectory(
            Path.Combine(DirectoryHandler.GetPluginDirectory(), "ChatAssistantPlugin"));

        try
        {
            var res = _kernel.InvokeStreamingAsync(plugin["Assistant"],
                new KernelArguments() { ["context"] = context, ["query"] = message.Message });
            await StreamAsync(message.ConnectionId, res);
        }
        catch (Exception ex)
        {
            await Clients.Client(message.ConnectionId)
                .SendAsync("ReceiveMessage", "Error", ex.Message);
        }
    }

    private async Task StreamAsync(string connectionId, IAsyncEnumerable<StreamingKernelContent> res)
    {
        var first = true;
        await foreach (var content in res.ConfigureAwait(false))
        {
            if (first)
            {
                await Clients.Client(connectionId)
                    .SendAsync("ReceiveMessage", "Assistatnt", "");
                first = false;
            }

            await Clients.Client(connectionId)
                .SendAsync("ReceiveMessage", "", content.ToString());
        }
    }
}
