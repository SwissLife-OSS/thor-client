using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A transmission sender for <c>Azure</c> <c>BLOB</c> <c>Storage</c>.
    /// </summary>
    public class BlobStorageTransmissionSender
        : ITransmissionSender<AttachmentDescriptor>
    {
        private readonly CloudBlobContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageTransmissionSender"/> class.
        /// </summary>
        /// <param name="container">A <c>Azure</c> <c>BLOB</c> <c>Storage</c> container instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="container"/> must not be <c>null</c>.
        /// </exception>
        public BlobStorageTransmissionSender(CloudBlobContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <inheritdoc/>
        public async Task SendAsync(AttachmentDescriptor[] batch, CancellationToken cancellationToken)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            if (batch.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batch), ExceptionMessages.CollectionIsEmpty);
            }

            try
            {
                foreach (AttachmentDescriptor descriptor in batch)
                {
                    await _container.GetBlockBlobReference($"{descriptor.Id}\\{descriptor.TypeName}")
                        .UploadFromByteArrayAsync(descriptor.Value, 0, descriptor.Value.Length)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                // todo: log via event provider
            }
        }
    }
}