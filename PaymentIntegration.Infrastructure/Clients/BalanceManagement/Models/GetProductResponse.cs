namespace PaymentIntegration.Infrastructure.Clients.BalanceManagement.Models;

public class GetProductResponse
{
    public List<GetProductList> Items { get; set; } = [];
}

public class GetProductList
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public string Category { get; set; }
    public int Stock { get; set; }
}