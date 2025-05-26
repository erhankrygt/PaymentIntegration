namespace PaymentIntegration.Api.Models.Responses;

public class CreateOrderResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public CreateOrderData Data { get; set; }
}

public class CreateOrderData
{
    public PreOrder PreOrder { get; set; }
    public UpdatedBalance UpdatedBalance { get; set; }
}

public class PreOrder
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    public string Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}

public class UpdatedBalance
{
    public string UserId { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public decimal BlockedBalance { get; set; }
    public string Currency { get; set; }
    public DateTime LastUpdated { get; set; }
}
