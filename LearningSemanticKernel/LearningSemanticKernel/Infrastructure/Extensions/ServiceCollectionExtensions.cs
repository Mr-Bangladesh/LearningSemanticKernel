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
        return services;
    }
}
