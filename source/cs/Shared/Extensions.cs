using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TrainsPlatform.Services;
using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.Services
{
    public static class EnumerableExt
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            var nextbatch = new List<T>(batchSize);
            foreach (T item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSize)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(batchSize);
                }
            }

            if (nextbatch.Count > 0)
            {
                yield return nextbatch;
            }
        }
    }

    public static class TrainPlatformSharedExtensions
    {
        public static void AddTrainPlatformShared(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StorageOptions>(configuration);
            services.AddSingleton<TrainEventsRepository>();
        }
    }
}
