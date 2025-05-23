using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using PaymentIntegration.Domain.Models;
using PaymentIntegration.Infrastructure.Clients.BalanceManagement;

namespace PaymentIntegration.IntegrationTests;

// IntegrationTests/OrdersControllerTests.cs
public class OrdersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OrdersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IBalanceManagementClient));
                
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var mockClient = new Mock<IBalanceManagementClient>();
                mockClient.Setup(x => x.GetProductsAsync())
                    .ReturnsAsync(new List<Product>
                    {
                        new() { Id = "1", Name = "Test Product", Price = 10, Stock = 10 }
                    });
                
                mockClient.Setup(x => x.CreatePreorderAsync(It.IsAny<decimal>()))
                    .ReturnsAsync("test-transaction");
                
                services.AddSingleton(mockClient.Object);
            });
        });
    }

    [Fact]
    public async Task CreateOrder_ReturnsCreatedResponse()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            Items = new[]
            {
                new { ProductId = "1", Quantity = 2 }
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/orders/create", request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}