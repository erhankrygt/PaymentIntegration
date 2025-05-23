// PaymentIntegration.Application/DependencyInjection.cs

using Microsoft.Extensions.DependencyInjection;
using PaymentIntegration.Application.Interfaces;
using PaymentIntegration.Application.Services;

namespace PaymentIntegration.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
            return services;
        }
    }
}