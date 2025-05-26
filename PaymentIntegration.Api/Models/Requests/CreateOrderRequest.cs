namespace PaymentIntegration.Api.Models.Requests;

public record CreateOrderRequest(decimal Amount, string OrderId);