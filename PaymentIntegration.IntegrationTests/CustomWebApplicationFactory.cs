using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using PaymentIntegration.Api;

namespace PaymentIntegration.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            // Add in-memory configuration for JWT settings for tests
            configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {"Jwt:Key", "your-super-secret-key-with-at-least-32-characters"},
                    {"Jwt:Issuer", "PaymentIntegration"},
                    {"Jwt:Audience", "PaymentIntegrationUsers"},
                    {"Jwt:ExpiryInMinutes", "60"}
                });
        });

        builder.UseEnvironment("Development"); // Or "IntegrationTest"

        base.ConfigureWebHost(builder);
    }
} 