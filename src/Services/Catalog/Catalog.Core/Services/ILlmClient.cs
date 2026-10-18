namespace Catalog.Core.Services;

public interface ILlmClient
{
    Task<string> CompleteAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken = default);
}

public class LlmException(string message, Exception? innerException = null) : Exception(message, innerException);
