namespace PaymentIntegration.Api.Models.Auth;

public class LoginResponse
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
} 