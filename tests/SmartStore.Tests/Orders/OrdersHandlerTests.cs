using BuildingBlocks.Events;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Orders.Application.Commands;
using Orders.Application.Exceptions;
using Orders.Application.Handlers;
using Orders.Core.Entities;
using Orders.Core.Repositories;
using Orders.Core.Services;

namespace SmartStore.Tests.Orders;

public class OrdersHandlerTests
{
    private const string ProductId = "6ad46e076035676a14a0488a";

    private readonly Mock<ICatalogClient> _catalog = new();
    private readonly Mock<IOrderRepository> _repository = new();

    private readonly Mock<IPublishEndpoint> _publish = new();

    private CreateOrderHandler CreateHandler() =>
        new(_catalog.Object, _repository.Object, _publish.Object, NullLogger<CreateOrderHandler>.Instance);

    [Fact]
    public async Task CreateOrder_SavesPendingOrderWithCatalogPrice_AndPublishesOrderPlaced()
    {
        _catalog
            .Setup(c => c.GetProductAsync(ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CatalogProduct(ProductId, "Yoga Mat", 35.00m, 200));

        var result = await CreateHandler().Handle(new CreateOrderCommand(ProductId, 3), CancellationToken.None);

        Assert.Equal("Pending", result.Status);
        Assert.Equal("Yoga Mat", result.ProductName);
        Assert.Equal(105.00m, result.TotalAmount);

        _repository.Verify(r => r.SaveAsync(
            It.Is<Order>(o => o.Status == OrderStatus.Pending && o.TotalAmount == 105.00m),
            It.IsAny<CancellationToken>()), Times.Once);

        _publish.Verify(p => p.Publish(
            It.Is<OrderPlaced>(e => e.OrderId == result.Id && e.Amount == 105.00m && e.Quantity == 3),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_Throws_WhenProductIsNotInCatalog_AndSavesAndPublishesNothing()
    {
        _catalog
            .Setup(c => c.GetProductAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatalogProduct?)null);

        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            CreateHandler().Handle(new CreateOrderCommand(ProductId, 1), CancellationToken.None));

        _repository.Verify(r => r.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        _publish.Verify(p => p.Publish(It.IsAny<OrderPlaced>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MarkOrderAsPaid_SetsStatusPaid_AndSaves()
    {
        var order = Order.Create(ProductId, "Yoga Mat", 35m, 1);
        _repository.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var handler = new MarkOrderAsPaidHandler(_repository.Object, NullLogger<MarkOrderAsPaidHandler>.Instance);
        var paidAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var updated = await handler.Handle(new MarkOrderAsPaidCommand(order.Id, Guid.NewGuid(), true, paidAt), CancellationToken.None);

        Assert.True(updated);
        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Equal(paidAt, order.PaidAt);
        _repository.Verify(r => r.SaveAsync(order, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkOrderAsPaid_LeavesOrderPending_WhenPaymentFailed()
    {
        var handler = new MarkOrderAsPaidHandler(_repository.Object, NullLogger<MarkOrderAsPaidHandler>.Instance);

        var updated = await handler.Handle(
            new MarkOrderAsPaidCommand(Guid.NewGuid(), Guid.NewGuid(), false, DateTime.UtcNow), CancellationToken.None);

        Assert.False(updated);
        _repository.Verify(r => r.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void Order_MarkAsPaid_IsIdempotent()
    {
        var order = Order.Create(ProductId, "Yoga Mat", 35m, 1);
        var first = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        order.MarkAsPaid(first);
        order.MarkAsPaid(first.AddHours(1));

        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Equal(first, order.PaidAt);
    }
}
