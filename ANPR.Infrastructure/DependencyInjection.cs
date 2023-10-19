using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Lonsum.Services.ANPR.Application.Repositories;
using Microsoft.Lonsum.Services.ANPR.Infrastructure.Data;
using Microsoft.Lonsum.Services.ANPR.Infrastructure.Repositories;

namespace Microsoft.Lonsum.Services.ANPR.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ANPRContext>(
                options => {
                    options.UseSqlServer(configuration["ConnectionString"],
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly("ANPR");
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        }
                    );
                },
                ServiceLifetime.Scoped //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            );

            services.AddScoped<IRecognizenEventRepository, RecognizenEventRepository>();

            return services;
        }
    }
}