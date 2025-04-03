using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.ChatCompletion;
#pragma warning disable SKEXP0001

namespace LearningSemanticKernel.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddAIComponents(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddKernel();
        return services
            .AddAnthropic(config);
    }

    private static IServiceCollection AddAnthropic(this IServiceCollection services, ConfigurationManager config)
    {
        var apiKey = config["Anthropic:APIKey"];

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentException("Anthropic API key is not set in the configuration.");
        }
        var skChatService =
            new ChatClientBuilder(new AnthropicClient(new APIAuthentication(apiKey)).Messages)
            .ConfigureOptions(opt =>
            {
                opt.ModelId = AnthropicModels.Claude35Haiku;
                opt.MaxOutputTokens = 1024;
            })
            .UseFunctionInvocation()
            .Build()
            .AsChatCompletionService();

        return services
            .AddSingleton<IChatCompletionService>(skChatService);
    }
}
