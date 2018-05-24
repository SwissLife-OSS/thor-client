using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A transmission storage for <c>Azure</c> <c>BLOB</c> <c>Storage</c>.
    /// </summary>
    public class BlobStorageTransmissionStorage
        : ITransmissionStorage<AttachmentDescriptor>
    {
        private readonly ConcurrentQueue<string> _attachments = new ConcurrentQueue<string>();
        private readonly string _storagePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageTransmissionStorage"/> class.
        /// </summary>
        /// <param name="storagePath">A storage path to save temporarily.</param>
        public BlobStorageTransmissionStorage(string storagePath)
        {
            if (string.IsNullOrWhiteSpace(storagePath))
            {
                throw new ArgumentNullException(nameof(storagePath));
            }

            _storagePath = storagePath;

            Initialize();
        }

        /// <inheritdoc/>
        public int Count => _attachments.Count;

        private void Initialize()
        {
            IEnumerable<FileInfo> files = Directory.CreateDirectory(_storagePath)
                .EnumerateFiles("*.tmp", SearchOption.TopDirectoryOnly);

            foreach (FileInfo file in files)
            {
                _attachments.Enqueue(file.FullName);
            }
        }

        /// <inheritdoc/>
        public Task<AttachmentDescriptor[]> DequeueAsync(CancellationToken cancellationToken)
        {
            AttachmentDescriptor[] batch = new AttachmentDescriptor[0];
            string fileName;

            if (_attachments.TryDequeue(out fileName))
            {
                string[] fileNameParts = Path.GetFileNameWithoutExtension(fileName).Split('_');

                batch = new[] 
                {
                    new AttachmentDescriptor
                    {
                        Id = fileNameParts[0],
                        Name = fileNameParts[1],
                        TypeName = fileNameParts[2],
                        Value = File.ReadAllBytes(fileName),
                    }
                };

                File.Delete(fileName);
            }

            return Task.FromResult(batch);
        }

        /// <inheritdoc/>
        public Task EnqueueAsync(AttachmentDescriptor[] batch, CancellationToken cancellationToken)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            if (batch.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batch), ExceptionMessages.CollectionIsEmpty);
            }

            foreach (AttachmentDescriptor descriptor in batch)
            {
                string fileName = Path.Combine(_storagePath, $"{descriptor.Id}_{descriptor.Name}_{descriptor.TypeName}.tmp");

                File.WriteAllBytes(fileName, descriptor.Value);
                _attachments.Enqueue(fileName);
            }

            return Task.FromResult(0);
        }
    }
}