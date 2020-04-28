using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A telemetry event transmitter for <c>Azure</c> <c>EventHub</c>.
    /// </summary>
    public sealed class EventHubTransmitter
        : ITelemetryEventTransmitter
        , IDisposable
    {
        private readonly CancellationTokenSource _disposeToken = new CancellationTokenSource();
        private readonly ITransmissionBuffer<EventData> _buffer;
        private readonly ITransmissionSender<EventData> _sender;
        private readonly ITransmissionStorage<EventData> _storage;
        private readonly Job _sendJob;
        private readonly Job _storeJob;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubTransmitter"/> class.
        /// </summary>
        /// <param name="buffer">A transmission buffer instance.</param>
        /// <param name="sender">A transmission sender instance.</param>
        /// <param name="storage">A transmission storage instance.</param>
        public EventHubTransmitter(
            ITransmissionBuffer<EventData> buffer,
            ITransmissionSender<EventData> sender,
            ITransmissionStorage<EventData> storage)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));

            _storeJob = Job.Start(
                async () => await StoreBatchAsync().ConfigureAwait(false),
                () => _buffer.Count == 0,
                _disposeToken.Token);

            _sendJob = Job.Start(
                async () => await SendBatchAsync().ConfigureAwait(false),
                () => _buffer.Count == 0,
                _disposeToken.Token);
        }

        /// <inheritdoc />
        public void Enqueue(TelemetryEvent data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!_disposeToken.IsCancellationRequested)
            {
                Task.Run(() => _buffer.EnqueueAsync(data.Map(), _disposeToken.Token));
            }
        }

        private async Task SendBatchAsync()
        {
            EventData[] batch = await _storage
                .DequeueAsync(_disposeToken.Token)
                .ConfigureAwait(false);

            if (batch.Length > 0)
            {
                await _sender
                    .SendAsync(batch, _disposeToken.Token)
                    .ConfigureAwait(false);
            }
        }

        private async Task StoreBatchAsync()
        {
            EventData[] batch = await _buffer
                .DequeueAsync(_disposeToken.Token)
                .ConfigureAwait(false);

            if (batch.Length > 0)
            {
                await _storage
                    .EnqueueAsync(batch.ToArray(), _disposeToken.Token)
                    .ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposeToken.Cancel();

                if (!_sendJob.Stopped && !_storeJob.Stopped)
                {
                    WaitHandle.WaitAll(new[]
                    {
                        _sendJob.WaitHandle,
                        _storeJob.WaitHandle
                    }, TimeSpan.FromSeconds(5));
                }

                _disposeToken?.Dispose();
                _sendJob?.Dispose();
                _storeJob?.Dispose();

                _disposed = true;
            }
        }
    }
}
