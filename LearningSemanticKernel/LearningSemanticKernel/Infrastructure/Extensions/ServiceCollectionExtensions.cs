using Amazon;
using Amazon.BedrockRuntime;
using Amazon.Runtime;
using Microsoft.SemanticKernel;
#pragma warning disable SKEXP0070
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
            .AddAmazonBedrock(config);
    }

    private static IServiceCollection AddAmazonBedrock(this IServiceCollection services, ConfigurationManager config)
    {
        var accessKey = config["AWS:AccessKey"];
        var secretKey = config["AWS:SecretKey"];
        var sessionToken = config["AWS:SessionToken"];
        var region = config["AWS:Region"];
        var modelId = config["AWS:ModelId"];

        var cred = new SessionAWSCredentials(accessKey, secretKey, sessionToken);
        var client = new AmazonBedrockRuntimeClient(cred, RegionEndpoint.GetBySystemName(region));

        return services
            .AddBedrockChatCompletionService(modelId, client);
    }
}
