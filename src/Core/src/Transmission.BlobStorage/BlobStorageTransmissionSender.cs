using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Thor.Core.Transmission.Abstractions;
using Thor.Core.Transmission.EventHub;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A transmission sender for <c>Azure</c> <c>BLOB</c> <c>Storage</c>.
    /// </summary>
    public class BlobStorageTransmissionSender
        : ITransmissionSender<AttachmentDescriptor>
    {
        private readonly IBlobContainer _container;
        private readonly Watcher<BlobStorageTransmissionSender> _watcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageTransmissionSender"/> class.
        /// </summary>
        /// <param name="container">A <c>BLOB</c> <c>Storage</c> container instance.</param>
        /// <param name="logger">A logger instance</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="container"/> must not be <c>null</c>.
        /// </exception>
        public BlobStorageTransmissionSender(
            IBlobContainer container,
            ILogger<BlobStorageTransmissionSender> logger)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _watcher = new Watcher<BlobStorageTransmissionSender>(logger);
        }

        /// <inheritdoc/>
        public async Task SendAsync(
            IAsyncEnumerable<AttachmentDescriptor> batch,
            CancellationToken cancellationToken)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            await foreach (AttachmentDescriptor attachment in batch.WithCancellation(cancellationToken))
            {
                try
                {
                    _watcher.Checkpoint();
                    await _container.UploadAsync(attachment, cancellationToken);
                }
                catch (Exception ex)
                {
                    _watcher.ReportError(ex);
                }
            }
        }
    }
}
