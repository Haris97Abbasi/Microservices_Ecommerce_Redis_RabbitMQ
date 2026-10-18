using Payments.Core.Entities;

namespace Payments.Application.Dtos;

public record PaymentDto(Guid Id, Guid OrderId, decimal Amount, string Status, DateTime ProcessedAt);

public static class PaymentMappingExtensions
{
    public static PaymentDto ToDto(this Payment payment) =>
        new(payment.Id, payment.OrderId, payment.Amount, payment.Status.ToString(), payment.ProcessedAt);
}
