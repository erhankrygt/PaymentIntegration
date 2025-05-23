using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentIntegration.Domain.Interfaces;
using PaymentIntegration.Infrastructure.Clients.BalanceManagement;
using PaymentIntegration.Infrastructure.Repositories;

namespace PaymentIntegration.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IBalanceManagementClient, BalanceManagementClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["BalanceManagement:BaseUrl"] ?? string.Empty);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddSingleton<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}