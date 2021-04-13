using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A transmission buffer for <c>Azure</c> <c>EventHub</c>.
    /// </summary>
    public class EventHubTransmissionBuffer
        : ITransmissionBuffer<EventData, EventDataBatch>
        , IDisposable
    {
        private static readonly int MaxBufferSize = 1000;
        private readonly CancellationTokenSource _disposeToken = new CancellationTokenSource();
        private readonly Channel<EventData> _input = Channel.CreateBounded<EventData>(MaxBufferSize);
        private readonly Channel<EventDataBatch> _output = Channel.CreateUnbounded<EventDataBatch>();
        private readonly EventHubProducerClient _client;
        private readonly Task _processingTask;
        private EventData _next;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubTransmissionBuffer"/> class.
        /// </summary>
        /// <param name="client">A <c>Azure</c> <c>EventHub</c> client instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="client"/> must not be <c>null</c>.
        /// </exception>
        public EventHubTransmissionBuffer(EventHubProducerClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _processingTask = TaskHelper
                .StartLongRunning(StartProcessing, _disposeToken.Token);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<EventDataBatch> Dequeue(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (await _output.Reader.WaitToReadAsync(cancellationToken))
            {
                while (_output.Reader.TryRead(out EventDataBatch eventsBatch))
                {
                    yield return eventsBatch;
                }
            }
        }

        /// <inheritdoc />
        public async Task Enqueue(
            IAsyncEnumerable<EventData> batch,
            CancellationToken cancellationToken)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            await foreach (EventData data in batch.WithCancellation(cancellationToken))
            {
                await Enqueue(data, cancellationToken);
            }
        }

        private async Task Enqueue(
            EventData data,
            CancellationToken cancellationToken)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            await _input.Writer.WriteAsync(data, cancellationToken);
        }

        private async Task StartProcessing()
        {
            while (await _input.Reader.WaitToReadAsync())
            {
                EventDataBatch batch = await _client.CreateBatchAsync();

                if (_next != null && batch.TryAdd(_next))
                {
                    _next = null;
                }

                while (_next == null && _input.Reader.TryRead(out EventData data))
                {
                    if (!batch.TryAdd(data))
                    {
                        _next = data;
                    }
                }

                await _output.Writer.WriteAsync(batch);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposeToken.Cancel();

                Task.WaitAll(new[]
                {
                    _processingTask,
                    _client.DisposeAsync().AsTask()
                }, TimeSpan.FromSeconds(5));
                
                _disposeToken?.Dispose();
                _disposed = true;
            }
        }
    }
}
