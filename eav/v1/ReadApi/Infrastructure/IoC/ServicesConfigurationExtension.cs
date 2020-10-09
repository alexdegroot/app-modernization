using Microsoft.Extensions.DependencyInjection;
using ReadApi.Domain.Repository;
using ReadApi.Infrastructure.Database;

namespace ReadApi.Infrastructure.IoC
{
    public static class ServicesConfigurationExtension
    {
        public static IServiceCollection ConfigureIoC(this IServiceCollection services)
        {
            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<IDatabaseReader, DatabaseReader>();
            return services;
        }
    }
}
