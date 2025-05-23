using PaymentIntegration.Domain.Models;

namespace PaymentIntegration.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(string id);
    Task<List<Order>> GetAllAsync();
    Task<Order> AddAsync(Order order);
    Task<Order> UpdateAsync(Order order);
}