using Catalog.Application.Commands;
using Catalog.Application.Dtos;
using Catalog.Application.Handlers;
using Catalog.Application.Queries;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Services;
using Moq;

namespace SmartStore.Tests.Catalog;

public class CatalogHandlerTests
{
    private readonly Mock<IProductRepository> _repository = new();

    [Fact]
    public async Task CreateProduct_SavesProductAndReturnsItWithGeneratedId()
    {
        _repository
            .Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => { p.Id = "6ad46e076035676a14a04886"; return p; });

        var handler = new CreateProductHandler(_repository.Object);
        var request = new ProductRequest("Desk Lamp", "LED lamp", "Home", 19.99m, 5);

        var result = await handler.Handle(new CreateProductCommand(request), CancellationToken.None);

        Assert.Equal("6ad46e076035676a14a04886", result.Id);
        Assert.Equal("Desk Lamp", result.Name);
        Assert.Equal(19.99m, result.Price);

        _repository.Verify(r => r.CreateAsync(
            It.Is<Product>(p => p.Name == "Desk Lamp" && p.Category == "Home" && p.Stock == 5),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsFalse_WhenProductDoesNotExist()
    {
        _repository
            .Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new UpdateProductHandler(_repository.Object);
        var request = new ProductRequest("X", "Y", "Z", 1m, 1);

        var updated = await handler.Handle(new UpdateProductCommand("000000000000000000000000", request), CancellationToken.None);

        Assert.False(updated);
    }

    [Fact]
    public void BuildSystemPrompt_ContainsEveryProductWithPriceAndStock()
    {
        var products = new List<Product>
        {
            new() { Id = "a1", Name = "Wireless Mouse", Category = "Electronics", Price = 24.99m, Stock = 150, Description = "Silent" },
            new() { Id = "b2", Name = "Yoga Mat", Category = "Sportswear", Price = 35m, Stock = 0, Description = "Non-slip" }
        };

        var prompt = AskAssistantHandler.BuildSystemPrompt(products);

        Assert.Contains("Wireless Mouse", prompt);
        Assert.Contains("price: $24.99", prompt);
        Assert.Contains("Yoga Mat", prompt);
        Assert.Contains("stock: 0", prompt);
        Assert.Contains("ONLY the product catalog", prompt);
    }

    [Fact]
    public void BuildSystemPrompt_SaysCatalogIsEmpty_WhenThereAreNoProducts()
    {
        var prompt = AskAssistantHandler.BuildSystemPrompt([]);

        Assert.Contains("(the catalog is currently empty)", prompt);
    }

    [Fact]
    public async Task AskAssistant_SendsCatalogAndQuestionToLlm_AndReturnsItsAnswer()
    {
        _repository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Product { Id = "a1", Name = "Running Shoes", Price = 120m }]);

        var llm = new Mock<ILlmClient>();
        llm.Setup(l => l.CompleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync("Yes, Running Shoes cost $120.00.");

        var handler = new AskAssistantHandler(_repository.Object, llm.Object);

        var response = await handler.Handle(new AskAssistantQuery("Do you sell shoes?"), CancellationToken.None);

        Assert.Equal("Yes, Running Shoes cost $120.00.", response.Answer);
        Assert.Equal(1, response.ProductsInContext);

        llm.Verify(l => l.CompleteAsync(
            It.Is<string>(system => system.Contains("Running Shoes")),
            "Do you sell shoes?",
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
