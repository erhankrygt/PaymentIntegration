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
    public async Task<List<Product>> GetAvailableProductsAsync()
    {
        try
        {
            return await balanceManagementClient.GetProductsAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get products from Balance Management");
            throw new BalanceManagementException("Failed to retrieve products", ex);
        }
    }

    public async Task<Order> CreateOrderAsync(List<OrderItem> items)
    {
        if (items == null || items.Count == 0)
            throw new OrderException("Order must contain at least one item");

        // Get product prices and validate
        var products = await balanceManagementClient.GetProductsAsync();
        var productDict = products.ToDictionary(p => p.Id);

        foreach (var item in items)
        {
            if (!productDict.TryGetValue(item.ProductId, out var product))
                throw new OrderException($"Product {item.ProductId} not found");

            if (item.Quantity <= 0)
                throw new OrderException($"Invalid quantity for product {item.ProductId}");

            if (product.Stock < item.Quantity)
                throw new OrderException($"Insufficient stock for product {item.ProductId}");

            item.ProductName = product.Name;
            item.UnitPrice = product.Price;
        }

        var totalAmount = items.Sum(i => i.Quantity * i.UnitPrice);

        // Reserve funds
        string transactionId;
        try
        {
            transactionId = await balanceManagementClient.CreatePreorderAsync(totalAmount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create preorder in Balance Management");
            throw new BalanceManagementException("Failed to reserve funds", ex);
        }

        // Create order
        var order = new Order
        {
            Items = items,
            TotalAmount = totalAmount,
            Status = OrderStatus.FundsReserved
        };

        order = await orderRepository.AddAsync(order);
        logger.LogInformation("Created order {OrderId} with transaction {TransactionId}", order.Id, transactionId);

        return order;
    }

    public async Task<Order> CompleteOrderAsync(string orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new OrderException($"Order {orderId} not found");

        if (order.Status != OrderStatus.FundsReserved)
            throw new OrderException($"Order {orderId} is not in a state that can be completed");

        try
        {
            await balanceManagementClient.CompleteOrderAsync(orderId);
            order.Status = OrderStatus.Completed;
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