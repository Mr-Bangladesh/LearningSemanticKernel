using System.Globalization;
using System.Linq;
using CsvHelper;
using LearningSemanticKernel.Entities;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Embeddings;
#pragma warning disable SKEXP0001
namespace LearningSemanticKernel.Services;

public class ProductCatalogService : IProductCatalogService
{
    private const string CsvFilePath = "Data/myntra_products_catalog.csv";
    private readonly ITextEmbeddingGenerationService _textEmbeddingGenerationService;
    private readonly IVectorStoreRecordCollection<string, ProductCatalog> _collection;

    public ProductCatalogService(ITextEmbeddingGenerationService textEmbeddingGenerationService,
        IVectorStoreRecordCollection<string, ProductCatalog> collection)
    {
        _textEmbeddingGenerationService = textEmbeddingGenerationService;
        _collection = collection;
    }

    public async Task ProcessCsvFileAsync()
    {
        var products = new List<ProductCatalog>();

        using (var reader = new StreamReader(CsvFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            // Map CSV columns to ProductCatalog properties
            var records = csv.GetRecords<ProductCatalog>();
            products.AddRange(records);
        }

        foreach (var product in products)
        {
            ProcessProductCatalog(product);
            await GenerateEmbeddingsAsync(product);
            await _collection.UpsertAsync(product);
        }
    }

    public async Task<string> FindRelatedContextDataAsync(string message)
    {
        // Generate an embedding from the search string.
        var searchVector = await _textEmbeddingGenerationService.GenerateEmbeddingAsync(message);

        // Search the store and get the single most relevant result.
        var searchResult = await _collection.VectorizedSearchAsync(
            searchVector,
            new()
            {
                Top = 1
            });
        var searchResultItems = await searchResult.Results.ToListAsync();

        return searchResultItems.First().Record.ProductData;
    }

    private static void ProcessProductCatalog(ProductCatalog product)
    {
        var data = @$"""
                    ProductId : {product.ProductId} \n
                    Name : {product.Name} \n
                    Brand: {product.Brand} \n
                    Gender: {product.Gender} \n
                    Price: {product.Price} \n
                    Description: {product.Description} \n
                    PrimaryColor: {product.PrimaryColor} \n
                   """;

        product.ProductData = data;
    }

    private async Task GenerateEmbeddingsAsync(ProductCatalog product)
    {
        product.ProductDataEmbedding = await _textEmbeddingGenerationService.GenerateEmbeddingAsync(product.ProductData);
    }
}
