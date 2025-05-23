// PaymentIntegration.Domain/DependencyInjection.cs

using Microsoft.Extensions.DependencyInjection;

namespace PaymentIntegration.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            return services;
        }
    }
}