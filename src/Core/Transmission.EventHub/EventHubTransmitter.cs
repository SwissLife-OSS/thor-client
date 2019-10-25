using System;
using System.Collections.Generic;
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
        private static readonly TimeSpan _delay = TimeSpan.FromMilliseconds(50);
        private readonly CancellationTokenSource _disposeToken = new CancellationTokenSource();
        private readonly ITransmissionBuffer<EventData> _buffer;
        private readonly ITransmissionSender<EventData> _sender;
        private bool _disposed;
        private Task _transmission;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubTransmitter"/> class.
        /// </summary>
        /// <param name="client">A <c>Azure</c> <c>EventHub</c> client instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="client"/> must not be <c>null</c>.
        /// </exception>
        public EventHubTransmitter(EventHubClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _buffer = new EventHubTransmissionBuffer(client);
            _sender = new EventHubTransmissionSender(client);

            StartAsyncSending();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubTransmitter"/> class.
        /// </summary>
        /// <param name="buffer">A transmission buffer instance.</param>
        /// <param name="sender">A transmission sender instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> must not be <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sender"/> must not be <c>null</c>.
        /// </exception>
        internal EventHubTransmitter(ITransmissionBuffer<EventData> buffer,
            ITransmissionSender<EventData> sender)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));

            StartAsyncSending();
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
                Task.Run(() => _buffer.EnqueueAsync(data.Map()));
            }
        }

        private async Task SendBatchAsync()
        {
            IEnumerable<EventData> batch = await _buffer
                .DequeueAsync()
                .ConfigureAwait(false);

            if (batch.Any())
            {
                await _sender
                    .SendAsync(batch)
                    .ConfigureAwait(false);
            }
        }

        private void StartAsyncSending()
        {
            _transmission = Task.Run(async () =>
            {
                while (!_disposeToken.IsCancellationRequested)
                {
                    await SendBatchAsync().ConfigureAwait(false);

                    if (!_disposeToken.IsCancellationRequested && _buffer.Count == 0)
                    {
                        await Task.Delay(_delay).ConfigureAwait(false);
                    }
                }
            });
        }

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _transmission.GetAwaiter().OnCompleted(() =>
                {
                    _disposeToken?.Dispose();
                });

                _disposeToken.Cancel();
                _disposed = true;
            }
        }

        #endregion
    }
}