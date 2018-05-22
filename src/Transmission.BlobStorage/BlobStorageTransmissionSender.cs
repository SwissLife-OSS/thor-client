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
        private readonly CloudBlobClient _client;
        private readonly BlobStorageConfiguration _configuration;
        private CloudBlobContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageTransmissionSender"/> class.
        /// </summary>
        /// <param name="client">A <c>Azure</c> <c>BLOB</c> <c>Storage</c> client instance.</param>
        /// <param name="configuration">A </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="client"/> must not be <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configuration"/> must not be <c>null</c>.
        /// </exception>
        public BlobStorageTransmissionSender(CloudBlobClient client,
            BlobStorageConfiguration configuration)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                _container = _client.GetContainerReference(_configuration.AttachmentContainerName);
                _container.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                // todo: log
            }
        }

        /// <inheritdoc/>
        public async Task SendAsync(AttachmentDescriptor[] batch, CancellationToken cancellationToken)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            if (batch.Length != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(batch),
                    ExceptionMessages.CollectionOneItem);
            }

            try
            {
                AttachmentDescriptor descriptor = batch[0];

                await _container.GetBlockBlobReference($"{descriptor.Id}\\{descriptor.TypeName}")
                    .UploadFromByteArrayAsync(descriptor.Value, 0, descriptor.Value.Length)
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                // todo: log
            }
        }
    }
}