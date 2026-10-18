using BuildingBlocks.Events;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Payments.Application.Commands;
using Payments.Application.Handlers;
using Payments.Core.Entities;
using Payments.Core.Repositories;

namespace SmartStore.Tests.Payments;

public class PaymentsHandlerTests
{
    private readonly Mock<IPaymentRepository> _repository = new();
    private readonly Mock<IPublishEndpoint> _publish = new();

    private ProcessPaymentHandler CreateHandler() =>
        new(_repository.Object, _publish.Object, NullLogger<ProcessPaymentHandler>.Instance);

    [Fact]
    public async Task ProcessPayment_StoresSucceededPayment_AndPublishesPaymentProcessed()
    {
        var orderId = Guid.NewGuid();
        _repository
            .Setup(r => r.GetByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        var result = await CreateHandler().Handle(new ProcessPaymentCommand(orderId, 699.98m), CancellationToken.None);

        Assert.Equal("Succeeded", result.Status);
        Assert.Equal(699.98m, result.Amount);

        _repository.Verify(r => r.AddAsync(
            It.Is<Payment>(p => p.OrderId == orderId && p.Status == PaymentStatus.Succeeded),
            It.IsAny<CancellationToken>()), Times.Once);

        _publish.Verify(p => p.Publish(
            It.Is<PaymentProcessed>(e => e.OrderId == orderId && e.Success && e.Amount == 699.98m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPayment_DoesNotChargeTwice_ForDuplicateOrderPlaced_ButRepublishes()
    {
        var existing = Payment.CreateSimulatedSuccess(Guid.NewGuid(), 50m);
        _repository
            .Setup(r => r.GetByOrderIdAsync(existing.OrderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await CreateHandler().Handle(new ProcessPaymentCommand(existing.OrderId, 50m), CancellationToken.None);

        Assert.Equal(existing.Id, result.Id);

        _repository.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);

        _publish.Verify(p => p.Publish(
            It.Is<PaymentProcessed>(e => e.PaymentId == existing.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
