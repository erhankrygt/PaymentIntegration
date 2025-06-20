using System.ComponentModel.DataAnnotations;

namespace PaymentIntegration.Api.Models.Auth;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
} 