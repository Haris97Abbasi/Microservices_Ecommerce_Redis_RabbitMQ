using System.Globalization;
using System.Text;
using Catalog.Application.Dtos;
using Catalog.Application.Queries;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Services;
using MediatR;

namespace Catalog.Application.Handlers;

public class AskAssistantHandler(IProductRepository repository, ILlmClient llmClient)
    : IRequestHandler<AskAssistantQuery, AssistantResponse>
{
    public async Task<AssistantResponse> Handle(AskAssistantQuery query, CancellationToken cancellationToken)
    {
        var products = await repository.GetAllAsync(cancellationToken);
        var systemPrompt = BuildSystemPrompt(products);

        var answer = await llmClient.CompleteAsync(systemPrompt, query.Question, cancellationToken);

        return new AssistantResponse(query.Question, answer, products.Count);
    }

    public static string BuildSystemPrompt(IReadOnlyList<Product> products)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are the SmartStore product assistant.");
        sb.AppendLine("Answer the customer's question using ONLY the product catalog below.");
        sb.AppendLine("If the catalog does not contain the answer, say so - never invent products, prices or stock.");
        sb.AppendLine("Mention product names and prices (USD) when relevant. Keep answers short.");
        sb.AppendLine();
        sb.AppendLine("PRODUCT CATALOG:");

        if (products.Count == 0)
            sb.AppendLine("(the catalog is currently empty)");

        foreach (var p in products)
        {
            sb.AppendLine(string.Create(CultureInfo.InvariantCulture,
                $"- {p.Name} | id: {p.Id} | category: {p.Category} | price: ${p.Price:0.00} | stock: {p.Stock} | {p.Description}"));
        }

        return sb.ToString();
    }
}
