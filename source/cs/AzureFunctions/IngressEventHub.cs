using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public static class IngressEventHub
    {
        [Function("IngressEventHub")]
        public static void Run([EventHubTrigger("samples-workitems", Connection = "")] string[] input, FunctionContext context)
        {
            var logger = context.GetLogger("IngressEventHub");
            logger.LogInformation($"First Event Hubs triggered message: {input[0]}");
        }
    }
}
