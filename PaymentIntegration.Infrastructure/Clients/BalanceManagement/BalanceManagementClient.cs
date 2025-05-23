using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PaymentIntegration.Domain.Exceptions;
using PaymentIntegration.Domain.Models;
using Polly;
using Polly.Retry;


namespace PaymentIntegration.Infrastructure.Clients.BalanceManagement;

public abstract class BalanceManagementClient : IBalanceManagementClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BalanceManagementClient> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    protected BalanceManagementClient(HttpClient httpClient, ILogger<BalanceManagementClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        // Configure retry policy with exponential backoff
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, delay, retryCount, context) =>
                {
                    _logger.LogWarning(exception,
                        $"Retry {retryCount} of {context.PolicyKey} due to: {exception.Message}. Waiting {delay} before next retry.");
                });
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync("/products");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<BalanceManagementProductsResponse>();
            return content?.Products.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock
            }).ToList() ?? new List<Product>();
        });
    }

    public async Task<string> CreatePreorderAsync(decimal amount)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var request = new { amount };
            var response = await _httpClient.PostAsJsonAsync("/preorder", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<BalanceManagementPreorderResponse>();
            return result?.TransactionId ?? throw new BalanceManagementException("Failed to get transaction ID");
        });
    }

    public async Task CompleteOrderAsync(string transactionId)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            var request = new { transactionId };
            var response = await _httpClient.PostAsJsonAsync("/complete", request);
            response.EnsureSuccessStatusCode();
        });
    }

    private record BalanceManagementProductsResponse(List<BalanceManagementProduct> Products);

    private record BalanceManagementProduct(string Id, string Name, decimal Price, int Stock);

    private record BalanceManagementPreorderResponse(string TransactionId);
}