using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TrainsPlatform.ConsoleLocal.Infrastructure;
using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs;
using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Abstractions;
using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Models;
using TrainsPlatform.ConsoleLocal.Infrastructure.Models;
using TrainsPlatform.Services;
using TrainsPlatform.Shared.Models;

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
            var useRealPlatform = _args.Contains("--use-real");
            useRealPlatform = true;
            var host = Host
                .CreateDefaultBuilder(_args)
#if DEBUG
                .UseEnvironment("Development")
#endif
                .ConfigureServices((context, services) =>
                {
                    services.Configure<TrainEventsReaderOptions>(context.Configuration);

                    services.AddTrainPlatformShared(context.Configuration);
                    if (useRealPlatform)
                    {
                        services.Configure<EventHubOptions>(context.Configuration);
                        services.AddSingleton<IEventHubFactory, EventHubRealFactory>();
                    }
                    else
                    {
                        services.AddSingleton<IEventHubFactory, EventHubSimulatorFactory>();
                    }
                    services.AddSingleton<TrainEventsReader>();
                });

            var built = host.Build();

            var reader = built.Services.GetRequiredService<TrainEventsReader>();

            var buffer = built.Services.GetRequiredService<IEventHubFactory>()
                .GetClientEventsEventHub();

            var trainEventsRepository = built.Services.GetRequiredService<TrainEventsRepository>();

            var eventReadingTask = Task.Run(async () =>
            {
                await foreach (var item in buffer.ReadEventsAsync(cancellationToken))
                {
                    Console.WriteLine($"{item.VehicleId}-{item.StationId}-{item.DateTime}");
                }
            }, cancellationToken);

            var loadingTask = Task.Run(async () => await reader.ReadEventsToBufferAsync(cancellationToken), cancellationToken);




            await Task.WhenAll(eventReadingTask, loadingTask);
        }
    }
}
