namespace Catalog.Infrastructure.Llm;

public class LlmSettings
{
    public const string SectionName = "Llm";

    public string BaseUrl { get; set; } = "https://api.openai.com/v1";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4o-mini";
    public int TimeoutSeconds { get; set; } = 60;
}
