using Catalog.Application.Dtos;
using MediatR;

namespace Catalog.Application.Queries;

public record AskAssistantQuery(string Question) : IRequest<AssistantResponse>;
