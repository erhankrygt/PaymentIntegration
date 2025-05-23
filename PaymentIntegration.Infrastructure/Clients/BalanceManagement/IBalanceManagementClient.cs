using PaymentIntegration.Domain.Models;

namespace PaymentIntegration.Infrastructure.Clients.BalanceManagement;

public interface IBalanceManagementClient
{
    Task<List<Product>> GetProductsAsync();
    Task<string> CreatePreorderAsync(decimal amount);
    Task CompleteOrderAsync(string transactionId);
}