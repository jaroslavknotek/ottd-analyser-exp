using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TrainsPlatform.Infrastructure.Azure;
using TrainsPlatform.Shared.Factories;

namespace TrainsPlatform.Services
{
  
    public static class TrainPlatformSharedExtensions
    {
        public static void AddTrainPlatformShared(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<TrainEventsRepository>();
            services.AddSingleton<InfrastructureFactory>();
        }
    }
}
