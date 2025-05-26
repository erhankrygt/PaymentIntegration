using PaymentIntegration.Infrastructure.Clients.BalanceManagement.Models;

namespace PaymentIntegration.Infrastructure.Clients.BalanceManagement;

public interface IBalanceManagementClient
{
    Task<GetProductResponse?> GetProductsAsync();
    Task<CreatePreorderResponse> CreatePreorderAsync(decimal amount, string orderId);
    Task CompleteOrderAsync(string transactionId);
}
