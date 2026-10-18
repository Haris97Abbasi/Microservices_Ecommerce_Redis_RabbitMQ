using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Catalog.Core.Services;
using Microsoft.Extensions.Options;

namespace Catalog.Infrastructure.Llm;

public class OpenAiCompatibleLlmClient(HttpClient httpClient, IOptions<LlmSettings> options) : ILlmClient
{
    public async Task<string> CompleteAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken = default)
    {
        var request = new ChatRequest(
            options.Value.Model,
            [new ChatMessage("system", systemPrompt), new ChatMessage("user", userMessage)],
            Temperature: 0.2);

        try
        {
            using var content = JsonContent.Create(request);
            await content.LoadIntoBufferAsync();

            using var response = await httpClient.PostAsync("chat/completions", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new LlmException($"LLM provider returned {(int)response.StatusCode}: {error}");
            }

            var body = await response.Content.ReadFromJsonAsync<ChatResponse>(cancellationToken);
            var answer = body?.Choices.FirstOrDefault()?.Message.Content;

            return string.IsNullOrWhiteSpace(answer)
                ? throw new LlmException("LLM provider returned an empty answer")
                : answer.Trim();
        }
        catch (HttpRequestException ex)
        {
            throw new LlmException($"Could not reach LLM provider: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new LlmException("LLM provider timed out", ex);
        }
    }

    private record ChatRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] IReadOnlyList<ChatMessage> Messages,
        [property: JsonPropertyName("temperature")] double Temperature);

    private record ChatMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content);

    private record ChatResponse(
        [property: JsonPropertyName("choices")] IReadOnlyList<ChatChoice> Choices);

    private record ChatChoice(
        [property: JsonPropertyName("message")] ChatMessage Message);
}
