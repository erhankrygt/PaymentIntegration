using System.Collections.Concurrent;
using PaymentIntegration.Domain.Interfaces;
using PaymentIntegration.Domain.Models;


namespace PaymentIntegration.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<string, Order> _orders = new();

    public Task<Order?> GetByIdAsync(string id)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<Order> AddAsync(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        if (order.OrderId != null) _orders[order.OrderId] = order;
        return Task.FromResult(order);
    }

    public Task<Order> UpdateAsync(Order order)
    {
        if (order.OrderId != null && !_orders.ContainsKey(order.OrderId))
            throw new KeyNotFoundException($"Order with ID {order.OrderId} not found");

        if (order.OrderId != null) _orders[order.OrderId] = order;
        return Task.FromResult(order);
    }
}