using System.Threading.Tasks;

using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using TrainsPlatform.Services;

namespace TrainsPlatform.AzureFunctions
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureHostConfiguration(
                        configureDelegate => configureDelegate.AddEnvironmentVariables())
                .ConfigureServices((context, services) =>
                {
                    services.AddTrainPlatformShared(context.Configuration);
                })
                .Build();

            host.Run();
        }
    }
}
