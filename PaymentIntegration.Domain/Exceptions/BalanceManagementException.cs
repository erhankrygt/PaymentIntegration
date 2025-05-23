namespace PaymentIntegration.Domain.Exceptions;

public class BalanceManagementException: Exception
{
    public BalanceManagementException(string message) : base(message) { }
    public BalanceManagementException(string message, Exception inner) : base(message, inner) { }
}