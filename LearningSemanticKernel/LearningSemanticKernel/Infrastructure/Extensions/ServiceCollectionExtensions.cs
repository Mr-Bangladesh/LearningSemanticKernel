using LearningSemanticKernel.Entities;
using LearningSemanticKernel.Services;
using Microsoft.SemanticKernel;
#pragma warning disable SKEXP0010
namespace LearningSemanticKernel.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IProductCatalogService, ProductCatalogService>();
    }

    public static IServiceCollection AddAIComponents(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddKernel();
        return services
            .AddOpenAI(config)
            .AddPostgreSQLVectorStore(config);
    }

    private static IServiceCollection AddOpenAI(this IServiceCollection services, ConfigurationManager config)
    {
        var apiKey = config["OpenAI:ApiKey"] ?? string.Empty;
        var modelId = config["OpenAI:ModelId"] ?? string.Empty;
        return services
            .AddOpenAIChatCompletion(modelId, apiKey)
            .AddOpenAITextEmbeddingGeneration(modelId, apiKey);
    }

    private static IServiceCollection AddPostgreSQLVectorStore(this IServiceCollection services, ConfigurationManager config)
    {
        var connectionString = config["PostgreSQL:ConnectionString"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("PostgreSQL connection string is not set in the configuration.");
        }

        return services
            .AddPostgresVectorStore(connectionString)
            .AddPostgresVectorStoreRecordCollection<string, ProductCatalog>("ProductCatalog");
    }
}
