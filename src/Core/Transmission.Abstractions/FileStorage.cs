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
        private static readonly TData[] EmptyBatch = new TData[0];
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
        public async Task<IReadOnlyCollection<TData>> DequeueAsync(
            int count,
            CancellationToken cancellationToken)
        {
            if (!HasData)
            {
                return EmptyBatch;
            }

            var batch = new List<TData>();

            foreach(FileInfo file in _files.Take(count))
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FullName);

                var memoryMapFile = new MemoryMap<TData>(fileName, file.FullName);

                TData data = await memoryMapFile
                    .LoadAsync(Deserialize, cancellationToken);

                if (data != null)
                {
                    batch.Add(data);
                    TryDelete(file.FullName);
                }
            }

            return batch;
        }

        /// <inheritdoc/>
        public async Task EnqueueAsync(
            IReadOnlyCollection<TData> batch,
            CancellationToken cancellationToken)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            foreach (TData data in batch)
            {
                await EnqueueAsync(data, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public Task EnqueueAsync(
            TData data,
            CancellationToken cancellationToken)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var fileName = EncodeFileName(data);
            var filePath = Path.Combine(_storagePath, $"{fileName}.tmp");

            var memoryMapFile = new MemoryMap<TData>(fileName, filePath);
            return memoryMapFile.CreateAsync(data, Serialize, cancellationToken);
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
