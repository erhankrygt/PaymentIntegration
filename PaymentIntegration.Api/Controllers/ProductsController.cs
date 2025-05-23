using Microsoft.AspNetCore.Mvc;
using PaymentIntegration.Application.Interfaces;
using PaymentIntegration.Domain.Exceptions;
using PaymentIntegration.Domain.Models;

namespace PaymentIntegration.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        try
        {
            var products = await orderService.GetAvailableProductsAsync();
            return Ok(products);
        }
        catch (BalanceManagementException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                Message = "Failed to retrieve products from payment service",
                Details = ex.Message
            });
        }
    }
}