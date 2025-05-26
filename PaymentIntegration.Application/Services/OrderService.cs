using Microsoft.Extensions.Logging;
using PaymentIntegration.Application.Interfaces;
using PaymentIntegration.Domain.Enums;
using PaymentIntegration.Domain.Exceptions;
using PaymentIntegration.Domain.Interfaces;
using PaymentIntegration.Domain.Models;
using PaymentIntegration.Infrastructure.Clients.BalanceManagement;

namespace PaymentIntegration.Application.Services;

public class OrderService(
    IBalanceManagementClient balanceManagementClient,
    IOrderRepository orderRepository,
    ILogger<OrderService> logger)
    : IOrderService
{
    public async Task<List<Product>?> GetAvailableProductsAsync()
    {
        try
        {
            var response = await balanceManagementClient.GetProductsAsync();
            var products = response?.Items
                .Select(item => new Product
                {
                    Name = item.Name,
                    Price = item.Price,
                    Stock = item.Stock,
                    Category = item.Category,
                    Currency = item.Currency
                })
                .ToList();

            return products;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get products from Balance Management");
            throw new BalanceManagementException("Failed to retrieve products", ex);
        }
    }

    public async Task<Order> CreateOrderAsync(decimal amount, string orderId)
    {
        try
        {
            var transactionId = Guid.NewGuid().ToString();
            var result = await balanceManagementClient.CreatePreorderAsync(amount, orderId);
            var or = result.Data.PreOrder;

            var order = new Order()
            {
                OrderId = or.OrderId,
                Amount = or.Amount,
                CanceledAt = or.CancelledAt,
                CompletedAt = or.CompletedAt,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                Status = (int)OrderStatus.Created
            };

           await orderRepository.AddAsync(order);
            
            logger.LogInformation("Created order {OrderId} with transaction {TransactionId}", order.OrderId,
                transactionId);
            return order;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create preorder in Balance Management");
            throw new BalanceManagementException("Failed to reserve funds", ex);
        }
    }

    public async Task<Order> CompleteOrderAsync(string orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new OrderException($"Order {orderId} not found");
        
        try
        {
            await balanceManagementClient.CompleteOrderAsync(orderId);
            order.Status = (int)OrderStatus.Completed;
            await orderRepository.UpdateAsync(order);

            logger.LogInformation("Completed order {OrderId}", orderId);
            return order;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to complete order {OrderId}", orderId);
            throw new BalanceManagementException("Failed to complete order", ex);
        }
    }

    public async Task<Order?> GetOrderByIdAsync(string id)
    {
        return await orderRepository.GetByIdAsync(id);
    }
}