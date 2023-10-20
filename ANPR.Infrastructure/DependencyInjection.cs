using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Lonsum.Services.ANPR.Application.Events;
using Microsoft.Lonsum.Services.ANPR.Application.Repositories;
using Microsoft.Lonsum.Services.ANPR.Infrastructure.Common.BuildingBlocks;
using Microsoft.Lonsum.Services.ANPR.Infrastructure.Data;
using Microsoft.Lonsum.Services.ANPR.Infrastructure.Events;
using Microsoft.Lonsum.Services.ANPR.Infrastructure.Repositories;
using RabbitMQ.Client;

namespace Microsoft.Lonsum.Services.ANPR.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddCustomDbContext(configuration)
                .AddCustomIntegration(configuration);

        }

        private static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ANPRContext>(
                options => {
                    options.UseSqlServer(configuration["ConnectionString"],
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(ANPRContext).Assembly.FullName);
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        }
                    );
                },
                ServiceLifetime.Scoped //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            );

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();


                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventBusConnection"],
                    DispatchConsumersAsync = true,
                    VirtualHost = configuration["EventBusVConnection"],
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            return services;
        }

        private static IServiceCollection AddCustomIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRecognizenEventRepository, RecognizenEventRepository>();
            services.AddSingleton<IEventBus, EventBusRabbitMQ>(
                sp =>
                {
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    string subscriptionName = configuration["SubscriptionClientName"];

                    return new EventBusRabbitMQ(rabbitMQPersistentConnection, subscriptionName);
                }
            );

            return services;
        }
    }
}