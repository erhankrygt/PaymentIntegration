using PaymentIntegration.Domain.Models;

namespace PaymentIntegration.Application.Interfaces;

public interface IOrderService
{
    Task<List<Product>> GetAvailableProductsAsync();
    Task<Order> CreateOrderAsync(List<OrderItem> items);
    Task<Order> CompleteOrderAsync(string orderId);
}