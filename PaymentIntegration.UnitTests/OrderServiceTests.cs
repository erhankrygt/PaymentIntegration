using Microsoft.Extensions.Logging;
using Moq;
using PaymentIntegration.Application.Services;
using PaymentIntegration.Domain.Enums;
using PaymentIntegration.Domain.Exceptions;
using PaymentIntegration.Domain.Interfaces;
using PaymentIntegration.Domain.Models;
using PaymentIntegration.Infrastructure.Clients.BalanceManagement;

namespace PaymentIntegration.UnitTests;

public class OrderServiceTests
{
    private readonly Mock<IBalanceManagementClient> _mockBalanceClient;
    private readonly Mock<IOrderRepository> _mockOrderRepo;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _mockBalanceClient = new Mock<IBalanceManagementClient>();
        _mockOrderRepo = new Mock<IOrderRepository>();
        _orderService = new OrderService(
            _mockBalanceClient.Object,
            _mockOrderRepo.Object,
            Mock.Of<ILogger<OrderService>>());
    }

    [Fact]
    public async Task CreateOrderAsync_ValidItems_CreatesOrder()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = "1", Name = "Product 1", Price = 10, Stock = 5 }
        };
        
        _mockBalanceClient.Setup(x => x.GetProductsAsync()).ReturnsAsync(products);
        _mockBalanceClient.Setup(x => x.CreatePreorderAsync(It.IsAny<decimal>()))
            .ReturnsAsync("trans123");
        
        var items = new List<OrderItem>
        {
            new() { ProductId = "1", Quantity = 2 }
        };

        // Act
        var order = await _orderService.CreateOrderAsync(items);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(20, order.TotalAmount);
        Assert.Equal(OrderStatus.FundsReserved, order.Status);
        _mockOrderRepo.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_InsufficientStock_ThrowsException()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = "1", Name = "Product 1", Price = 10, Stock = 1 }
        };
        
        _mockBalanceClient.Setup(x => x.GetProductsAsync()).ReturnsAsync(products);
        
        var items = new List<OrderItem>
        {
            new() { ProductId = "1", Quantity = 2 }
        };

        // Act & Assert
        await Assert.ThrowsAsync<OrderException>(() => _orderService.CreateOrderAsync(items));
    }
}