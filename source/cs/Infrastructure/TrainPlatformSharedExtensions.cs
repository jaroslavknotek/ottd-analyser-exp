
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TrainsPlatform.Infrastructure.Abstractions;
using TrainsPlatform.Infrastructure.Azure;
using TrainsPlatform.Infrastructure.InMemory;

namespace TrainsPlatform.Services
{

    public static class InfrastructureExtensions
    {
        public static void AddAzureInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AzureClientEventHubOptions>(configuration);
            services.Configure<AzureTrainEventsStorageOptions>(configuration);

            services.AddSingleton<IStorageTableFactory, AzureTableFactory>();
            services.AddSingleton<IEventHubFactory, AzureEventHubFactory>();
        }

        public static void AddInMemoryInfrastructure(this IServiceCollection services, IConfiguration _)
        {
            services.AddSingleton<IStorageTableFactory, InMemoryTableFactory>();
            services.AddSingleton<IEventHubFactory, InMemoryEventHubFactory>();

        }
    }
}
