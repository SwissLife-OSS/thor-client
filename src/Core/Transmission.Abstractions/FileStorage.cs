using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Thor.Core.Abstractions;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A transmission storage for <c>Files</c>.
    /// </summary>
    public abstract class FileStorage<TData>
        : ITransmissionStorage<TData>
        where TData : class
    {
        private static readonly int MaxBatchSize = 100;
        private readonly TData[] _emptyBatch = new TData[0];
        private readonly string _storagePath;
        private readonly IEnumerable<FileInfo> _files;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorage{TData}"/> class.
        /// </summary>
        /// <param name="storagePath">A storage path to save temporarily.</param>
        protected FileStorage(string storagePath)
        {
            if (string.IsNullOrWhiteSpace(storagePath))
            {
                throw new ArgumentNullException(nameof(storagePath));
            }

            _storagePath = storagePath;

            _files = Directory
                .CreateDirectory(_storagePath)
                .EnumerateFiles("*.tmp", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Create a <see cref="TData"/> object from <see cref="byte"/> array.
        /// </summary>
        protected abstract TData Deserialize(byte[] payload, string fileName);

        /// <summary>
        /// Create a <see cref="byte"/> array from <see cref="TData"/>.
        /// </summary>
        protected abstract byte[] Serialize(TData data);

        /// <summary>
        /// Generates a file name.
        /// </summary>
        protected abstract string EncodeFileName(TData data);

        /// <inheritdoc/>
        public bool HasData => _files.Any();

        /// <inheritdoc/>
        public async Task<TData[]> DequeueAsync(
            CancellationToken cancellationToken)
        {
            if (!HasData)
            {
                return _emptyBatch;
            }

            FileInfo[] files = _files.Take(MaxBatchSize).ToArray();
            var batch = new TData[files.Length];

            for (var index = 0; index < files.Length; index++)
            {
                FileInfo file = files[index];
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FullName);
                byte[] bytes = await FileHelper
                    .ReadAllBytesAsync(file.FullName, cancellationToken);

                batch[index] = Deserialize(bytes, fileNameWithoutExtension);

                TryDelete(file.FullName);
            }

            return batch;
        }

        /// <inheritdoc/>
        public async Task EnqueueAsync(
            TData[] batch,
            CancellationToken cancellationToken)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            if (batch.Length == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(batch), ExceptionMessages.CollectionIsEmpty);
            }

            foreach (TData data in batch)
            {
                var fileName = Path.Combine(_storagePath, $"{EncodeFileName(data)}.tmp");

                await FileHelper.WriteAllBytesAsync(
                    fileName,
                    Serialize(data),
                    cancellationToken);
            }
        }

        private void TryDelete(string fullName)
        {
            try
            {
                File.Delete(fullName);
            }
            catch (FileNotFoundException)
            {
                // Don't crash if file doesn't exists.
            }
        }
    }
}
