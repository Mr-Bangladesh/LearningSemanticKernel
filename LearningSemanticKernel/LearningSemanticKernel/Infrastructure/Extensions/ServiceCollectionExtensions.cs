using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070

namespace LearningSemanticKernel.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    private const string Llama32 = "llama3.2";
    //private const string Deepseek7b = "deepseek-llm:7b";
    private const string OllamaEndpoint = "http://localhost:11434";

    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddAIComponents(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddKernel();
        return services
            .AddOllama()
            .AddPostgreSQLVectorStore(config);
    }

    private static IServiceCollection AddOllama(this IServiceCollection services)
    {
        return services
            .AddOllamaChatCompletion(
                modelId: Llama32,
                endpoint: new Uri(OllamaEndpoint))
            .AddOllamaTextEmbeddingGeneration(
                modelId: Llama32,
                endpoint: new Uri(OllamaEndpoint));
    }

    private static IServiceCollection AddPostgreSQLVectorStore(this IServiceCollection services, ConfigurationManager config)
    {
        var connectionString = config["PostgreSQL:ConnectionString"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("PostgreSQL connection string is not set in the configuration.");
        }

        return services
            .AddPostgresVectorStore(connectionString);
            //.AddPostgresVectorStoreRecordCollection<string, Test>("Test");
    }
}
