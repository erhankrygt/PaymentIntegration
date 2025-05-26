using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PaymentIntegration.Api.Controllers;
using PaymentIntegration.Api.Models.Auth;
using Xunit;

namespace PaymentIntegration.UnitTests.Auth;

public class AuthControllerTests
{
    private readonly IConfiguration _configuration;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"Jwt:Key", "your-super-secret-key-with-at-least-32-characters"},
                {"Jwt:Issuer", "PaymentIntegration"},
                {"Jwt:Audience", "PaymentIntegrationUsers"},
                {"Jwt:ExpiryInMinutes", "60"}
            }!)
            .Build();

        _controller = new AuthController(_configuration);
    }

    [Fact]
    public void Login_WithValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "string"
        };

        // Act
        var result = _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.NotNull(response.Token);
        Assert.True(response.Expiration > DateTime.UtcNow);
    }

    [Fact]
    public void Login_WithInvalidCredentials_ReturnsUnauthorizedResult()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "wrong@example.com",
            Password = "WrongPassword"
        };

        // Act
        var result = _controller.Login(request);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
} 