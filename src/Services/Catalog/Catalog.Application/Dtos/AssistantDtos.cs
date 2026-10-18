using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.Dtos;

public record AssistantRequest([Required, StringLength(1000, MinimumLength = 3)] string Question);

public record AssistantResponse(string Question, string Answer, int ProductsInContext);
