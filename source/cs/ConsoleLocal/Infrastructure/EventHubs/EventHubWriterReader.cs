using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Abstractions;
using TrainsPlatform.Services;
using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs
{
    public class EventHubWriterReader<T>
        where T : new()
    {
        private readonly IEventHub _eventHub;

        public EventHubWriterReader(IEventHub eventHub)
        {
            _eventHub = eventHub;
        }

        public async Task WriteAsync(TrainEvent evnt, CancellationToken cancellationToken = default)
        {
            await WriteBatchAsync(new[] { evnt }, cancellationToken);
        }

        public async Task WriteBatchAsync(IEnumerable<TrainEvent> events, CancellationToken cancellationToken = default)
        {

            var batches = events.Batch(100);

            foreach (var batch in batches)
            {
                var items = events.Select(r =>
                {
                    var json = JsonSerializer.Serialize(r);
                    var bytes = Encoding.UTF8.GetBytes(json);
                    return new ReadOnlyMemory<byte>(bytes);
                });
                await _eventHub.WriteBatchAsync(items, cancellationToken);
            }
        }

        public async IAsyncEnumerable<T> ReadEventsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var bytes in _eventHub.ReadAsync(cancellationToken))
            {
                var json = Encoding.UTF8.GetString(bytes.Span);
                yield return JsonSerializer.Deserialize<T>(json);
            }
        }
    }
}
