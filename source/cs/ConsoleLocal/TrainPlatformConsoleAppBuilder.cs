using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs;
using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Models;
using TrainsPlatform.ConsoleLocal.Infrastructure.EventReader;
using TrainsPlatform.ConsoleLocal.Infrastructure.EventReader.Models;
using TrainsPlatform.Infrastructure.Abstractions;
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
                .GetClientEventsEventHub<TrainEvent>();

            var trainEventsRepository = built.Services.GetRequiredService<TrainEventsRepository>();

            var eventReadingTask = Task.Run(async () =>
            {
                await foreach (var item in buffer.ReadEventsAsync(cancellationToken))
                {
                    // not particulary effective
                    await trainEventsRepository.StoreAsync(new[] { item });
                }
            }, cancellationToken);

            var loadingTask = Task.Run(async () => await reader.ReadEventsToBufferAsync(cancellationToken), cancellationToken);

            await Task.WhenAll(eventReadingTask, loadingTask);
        }
    }
}
