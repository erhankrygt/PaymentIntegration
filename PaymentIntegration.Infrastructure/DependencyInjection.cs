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
            var url = new Uri(configuration["BalanceManagement:BaseUrl"] ?? string.Empty);
            services.AddHttpClient<IBalanceManagementClient, BalanceManagementClient>(client =>
            {
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddSingleton<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}