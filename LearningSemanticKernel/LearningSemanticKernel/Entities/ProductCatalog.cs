using Microsoft.Extensions.VectorData;

namespace LearningSemanticKernel.Entities;

public class ProductCatalog
{
    [VectorStoreRecordKey]
    public string Key { get; set; }

    [VectorStoreRecordData]
    public string ProductId { get; set; }

    [VectorStoreRecordData]
    public string Name { get; set; }

    [VectorStoreRecordData]
    public string Brand { get; set; }

    [VectorStoreRecordData]
    public string Gender { get; set; }

    [VectorStoreRecordData]
    public decimal Price { get; set; }

    [VectorStoreRecordData]
    public string Description { get; set; }

    [VectorStoreRecordData]
    public string PrimaryColor { get; set; }

    [VectorStoreRecordData]
    public string ProductData { get; set; }

    [VectorStoreRecordVector(Dimensions: 1536)]
    public ReadOnlyMemory<float> ProductDataEmbedding { get; set; }
}
