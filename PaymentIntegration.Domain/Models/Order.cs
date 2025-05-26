using PaymentIntegration.Domain.Enums;

namespace PaymentIntegration.Domain.Models;

public class Order : BaseEntity
{
    public int Status { get; set; }
    public decimal Amount { get; set; }
    public string? OrderId { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CanceledAt { get; set; }
}