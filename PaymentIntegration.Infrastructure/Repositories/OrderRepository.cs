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

    public Task<List<Order>> GetAllAsync()
    {
        return Task.FromResult(_orders.Values.ToList());
    }

    public Task<Order> AddAsync(Order order)
    {
        order.Id = Guid.NewGuid().ToString();
        order.CreatedAt = DateTime.UtcNow;
        _orders[order.Id] = order;
        return Task.FromResult(order);
    }

    public Task<Order> UpdateAsync(Order order)
    {
        if (!_orders.ContainsKey(order.Id))
            throw new KeyNotFoundException($"Order with ID {order.Id} not found");

        _orders[order.Id] = order;
        return Task.FromResult(order);
    }
}