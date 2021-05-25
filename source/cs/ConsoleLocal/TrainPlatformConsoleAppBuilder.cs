using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TrainsPlatform.ConsoleLocal.Infrastructure.EventReader;
using TrainsPlatform.ConsoleLocal.Infrastructure.EventReader.Models;
using TrainsPlatform.Infrastructure.Abstractions;
using TrainsPlatform.Infrastructure.Azure;
using TrainsPlatform.Infrastructure.InMemory;
using TrainsPlatform.Services;
using TrainsPlatform.Shared.Factories;

namespace TrainsPlatform.ConsoleLocal
{
    public class TrainPlatformConsoleAppBuilder
    {
        private readonly string[] _args;
        public TrainPlatformConsoleAppBuilder(string[] args)
        {
            _args = args;
        }
        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var useAzure = _args.Contains("--azure");
            useAzure = true;
            var host = Host
                .CreateDefaultBuilder(_args)
#if DEBUG
                .UseEnvironment("Development")
#endif
                .ConfigureServices((context, services) =>
                {
                    services.Configure<TrainEventsReaderOptions>(context.Configuration);

                    services.AddTrainPlatformShared(context.Configuration);
                    if (useAzure)
                    {
                        services.AddAzureInfrastructure(context.Configuration);
                    }
                    else
                    {
                        services.AddInMemoryInfrastructure(context.Configuration);
                    }
                    services.AddSingleton<TrainEventsReader>();
                });

            var built = host.Build();

            var reader = built.Services.GetRequiredService<TrainEventsReader>();

            var buffer = built.Services
                .GetRequiredService<InfrastructureFactory>()
                .GetClientEventsEventHub();

            var trainEventsRepository = built.Services.GetRequiredService<TrainEventsRepository>();

            var eventReadingTask = Task.Run(async () =>
            {
                await foreach (var item in buffer.ReadEventsAsync(cancellationToken))
                {
                    // not particulary effective
                    try
                    {
                        await trainEventsRepository.StoreAsync(new[] { item });
                        Console.WriteLine($"finished: {item.DateTime}");
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.Message);
                    }
                }
            }, cancellationToken);

            var loadingTask = Task.Run(async () => await reader.ReadEventsToBufferAsync(cancellationToken), cancellationToken);

            await Task.WhenAll(eventReadingTask, loadingTask);
        }
    }
}
