namespace Orders.Infrastructure.Catalog;

public class CatalogApiSettings
{
    public const string SectionName = "CatalogApi";

    public string BaseUrl { get; set; } = "http://localhost:5001";
    public int TimeoutSeconds { get; set; } = 10;
}
