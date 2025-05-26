using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PaymentIntegration.Domain.Exceptions;
using PaymentIntegration.Infrastructure.Clients.BalanceManagement.Models;
using Polly;
using Polly.Retry;


namespace PaymentIntegration.Infrastructure.Clients.BalanceManagement;

public class BalanceManagementClient : IBalanceManagementClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BalanceManagementClient> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public BalanceManagementClient(HttpClient httpClient, ILogger<BalanceManagementClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

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

    public async Task<GetProductResponse?> GetProductsAsync()
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync("/products");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<GetProductResponse>();
            return content;
        });
    }

    public async Task<CreatePreorderResponse> CreatePreorderAsync(decimal amount, string orderId)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var request = new { amount, orderId };
            var response = await _httpClient.PostAsJsonAsync("/preorder", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CreatePreorderResponse>();
            return result;
        }) ?? throw new BalanceManagementException("Failed to create preorder");
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
}