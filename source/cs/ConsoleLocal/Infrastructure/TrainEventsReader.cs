using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs;
using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Abstractions;
using TrainsPlatform.ConsoleLocal.Infrastructure.Models;
using TrainsPlatform.Services;
using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.ConsoleLocal.Infrastructure
{
    public class TrainEventsReader
    {
        private readonly string _eventDirectory;
        private readonly EventHubWriterReader<TrainEvent> _eventHubReaderWriter;

        public TrainEventsReader(
            IEventHubFactory eventHubFactory,
            IOptions<TrainEventsReaderOptions> readerOptions)
        {
            _eventDirectory = readerOptions.Value.EventDirectory;
            _eventHubReaderWriter = eventHubFactory.GetClientEventsEventHub();
        }

        public async Task ReadEventsToBufferAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var files = Directory.GetFiles(_eventDirectory);
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    var tempFile = GetTempDestination(fileInfo.Name);
                    await MoveSafeAsync(file, tempFile);

                    var lines = await File.ReadAllLinesAsync(tempFile, cancellationToken);

                    var events = lines
                        .Where(r => !string.IsNullOrEmpty(r))
                        .Select(r =>
                        {
                            return JsonSerializer.Deserialize<TrainEvent>(r)!;
                        });

                    await _eventHubReaderWriter.WriteBatchAsync(events, cancellationToken);


                    File.Delete(tempFile);
                }
            }
        }

        private static async Task MoveSafeAsync(string from, string to)
        {
            try
            {
                File.Move(from, to);
            }
            catch (IOException e) when (e.Message.Contains("exists"))
            {
                // temp is here to prevent loss of data 
                // if data gets read, then copied to file and then the file deleted
                // there can be some records written to the file at the time between
                // reading and deleting
                var temp = Path.GetTempFileName();
                File.Move(from, temp);
                var content = await File.ReadAllTextAsync(temp);
                File.AppendAllText(to, content);
            }
        }

        private string GetTempDestination(string file)
        {
            var directory = Path.Join(_eventDirectory, "temp", file);
            Directory.CreateDirectory(directory);
            var timestamp = DateTime.Now.ToString("s").Replace(':', '-');
            return Path.Join(directory, timestamp);
        }
    }
}
