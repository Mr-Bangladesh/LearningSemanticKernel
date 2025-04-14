
namespace LearningSemanticKernel.Services;

public interface IProductCatalogService
{
    Task ProcessCsvFileAsync();
    Task<string> FindRelatedContextDataAsync(string message);
}