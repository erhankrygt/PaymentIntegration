namespace PaymentIntegration.Domain.Exceptions;

public class OrderException(string message) : Exception(message);