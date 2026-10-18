using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notifications.Application.Commands;
using Notifications.Application.Handlers;
using Notifications.Core.Entities;
using Notifications.Core.Repositories;

namespace SmartStore.Tests.Notifications;

public class NotificationsHandlerTests
{
    private readonly Mock<INotificationRepository> _repository = new();

    private CreatePaymentNotificationHandler CreateHandler() =>
        new(_repository.Object, NullLogger<CreatePaymentNotificationHandler>.Instance);

    [Fact]
    public async Task CreateNotification_StoresSuccessMessage_ForNewPayment()
    {
        var orderId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        _repository
            .Setup(r => r.ExistsForPaymentAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var created = await CreateHandler().Handle(
            new CreatePaymentNotificationCommand(orderId, paymentId, true, 120m), CancellationToken.None);

        Assert.True(created);
        _repository.Verify(r => r.AddAsync(
            It.Is<Notification>(n =>
                n.OrderId == orderId &&
                n.PaymentId == paymentId &&
                n.Type == "PaymentSucceeded" &&
                n.Message.Contains("was successful")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateNotification_Skips_WhenPaymentWasAlreadyNotified()
    {
        _repository
            .Setup(r => r.ExistsForPaymentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var created = await CreateHandler().Handle(
            new CreatePaymentNotificationCommand(Guid.NewGuid(), Guid.NewGuid(), true, 120m), CancellationToken.None);

        Assert.False(created);
        _repository.Verify(r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
