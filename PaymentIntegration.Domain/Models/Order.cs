using PaymentIntegration.Domain.Enums;

namespace PaymentIntegration.Domain.Models;

public class Order
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}