namespace Catalog.Infrastructure.Persistence;

public class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "catalogdb";
    public string ProductsCollectionName { get; set; } = "products";
}
