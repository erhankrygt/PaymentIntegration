using PaymentIntegration.Domain.Models;

namespace PaymentIntegration.Application.Interfaces;

public interface IOrderService
{
    Task<List<Product>?> GetAvailableProductsAsync();
    Task<Order> CreateOrderAsync(decimal amount, string orderId);
    Task<Order> CompleteOrderAsync(string orderId);
}