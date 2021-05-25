using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrainsPlatform.ConsoleLocal
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var trains = new TrainPlatformConsoleAppBuilder(args);

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (_, _) => cts.Cancel();

            await trains.RunAsync(cts.Token);
        }
    }
}
