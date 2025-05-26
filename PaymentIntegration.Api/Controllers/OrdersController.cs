using Microsoft.AspNetCore.Mvc;
using PaymentIntegration.Api.Models.Requests;
using PaymentIntegration.Application.Interfaces;
using PaymentIntegration.Domain.Exceptions;
using PaymentIntegration.Domain.Models;

namespace PaymentIntegration.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var order = await orderService.CreateOrderAsync(request.Amount, request.OrderId);
            return Ok(order);
        }
        catch (OrderException ex)
        {
            logger.LogWarning(ex, "Order validation failed");
            return BadRequest(new { Message = ex.Message });
        }
        catch (BalanceManagementException ex)
        {
            logger.LogError(ex, "Balance Management error during order creation");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                Message = "Payment service unavailable",
                Details = ex.Message
            });
        }
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<Order>> CompleteOrder(string id)
    {
        try
        {
            var order = await orderService.CompleteOrderAsync(id);
            return Ok(order);
        }
        catch (OrderException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (BalanceManagementException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                Message = "Failed to complete payment",
                Details = ex.Message
            });
        }
    }
}