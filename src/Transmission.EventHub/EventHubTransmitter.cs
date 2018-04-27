using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Thor.Core.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A telemetry transmitter for <c>Azure</c> <c>EventHub</c>.
    /// </summary>
    public class EventHubTransmitter
        : ITelemetryTransmitter
    {
        private static readonly TimeSpan _delay = TimeSpan.FromMilliseconds(50);
        private readonly CancellationTokenSource _disposeToken = new CancellationTokenSource();
        private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim();
        private readonly ITransmissionBuffer<EventData> _buffer;
        private readonly ITransmissionSender<EventData> _sender;
        private bool _disposed = false;
        private Task _transmission;
        private bool _transmissionStopped = false;

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
        public void Enqueue(TelemetryEvent telemetryEvent)
        {
            if (telemetryEvent == null)
            {
                throw new ArgumentNullException(nameof(telemetryEvent));
            }

            if (!_disposeToken.IsCancellationRequested)
            {
                Task.Run(() => _buffer.EnqueueAsync(telemetryEvent.Map()));
            }
        }

        private async Task SendBatchAsync()
        {
            EventData[] batch;

            if (await _buffer.TryDequeueAsync(out batch).ConfigureAwait(false))
            {
                await _sender.SendAsync(batch).ConfigureAwait(false);
            }
        }

        private void StartAsyncSending()
        {
            _transmission = Task.Run(async () =>
            {
                while (!_disposeToken.IsCancellationRequested || _buffer.Count > 0)
                {
                    await SendBatchAsync().ConfigureAwait(false);

                    if (!_disposeToken.IsCancellationRequested && _buffer.Count == 0)
                    {
                        await Task.Delay(_delay).ConfigureAwait(false);
                    }
                }

                _transmissionStopped = true;
                _resetEvent.Set();
            });
        }

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposeToken.Cancel();

                if (!_transmissionStopped)
                {
                    _resetEvent.Wait(TimeSpan.FromSeconds(5));
                }

                _disposeToken?.Dispose();
                _resetEvent?.Dispose();
                _transmission?.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}