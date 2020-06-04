using System;
using System.Threading;
using System.Threading.Tasks;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A telemetry attachment transmitter for <c>Azure</c> <c>BLOB</c> <c>Storage</c>.
    /// </summary>
    public sealed class BlobStorageTransmitter
        : ITelemetryAttachmentTransmitter
        , IDisposable
    {
        private readonly CancellationTokenSource _disposeToken = new CancellationTokenSource();
        private readonly IMemoryBuffer<AttachmentDescriptor> _buffer;
        private readonly ITransmissionStorage<AttachmentDescriptor> _storage;
        private readonly ITransmissionSender<AttachmentDescriptor> _sender;
        private readonly Task _storeTask;
        private readonly Task _sendTask;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageTransmitter"/> class.
        /// </summary>
        public BlobStorageTransmitter(
            IMemoryBuffer<AttachmentDescriptor> buffer,
            ITransmissionStorage<AttachmentDescriptor> storage,
            ITransmissionSender<AttachmentDescriptor> sender)
        {
            _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));

            _storeTask = TaskHelper
                .StartLongRunning(StoreAsync, _disposeToken.Token);
            _sendTask = TaskHelper
                .StartLongRunning(SendAsync, _disposeToken.Token);
        }

        /// <inheritdoc/>
        public void Enqueue(AttachmentDescriptor data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!_disposeToken.IsCancellationRequested)
            {
                _buffer.Enqueue(data);
            }
        }

        private async Task StoreAsync()
        {
            await _storage
                .EnqueueAsync(_buffer.Dequeue(_disposeToken.Token), _disposeToken.Token)
                .ConfigureAwait(false);
        }

        private async Task SendAsync()
        {
            await _sender
                .SendAsync(_storage.DequeueAsync(_disposeToken.Token), _disposeToken.Token)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposeToken.Cancel();

                SpinWait.SpinUntil(() =>
                        _sendTask.Status != TaskStatus.Running &&
                        _storeTask.Status != TaskStatus.Running,
                    TimeSpan.FromSeconds(5));

                _disposeToken?.Dispose();
                _disposed = true;
            }
        }
    }
}
