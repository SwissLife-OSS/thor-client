using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A transmission storage for <c>Files</c>.
    /// </summary>
    public abstract class FileStorage<TData>
        : ITransmissionStorage<TData>
        where TData : class
    {
        private readonly string _storagePath;
        private readonly ChannelReader<string> _filesReader;
        private readonly ChannelWriter<string> _filesWriter;

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

            var files = Channel.CreateUnbounded<string>();
            _filesReader = files.Reader;
            _filesWriter = files.Writer;

            IEnumerable<FileInfo> unprocessedFiles = Directory
                .CreateDirectory(_storagePath)
                .EnumerateFiles("*.tmp", SearchOption.TopDirectoryOnly);

            foreach (FileInfo fileInfo in unprocessedFiles)
            {
                _filesWriter.TryWrite(fileInfo.FullName);
            }
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
        public async Task<IReadOnlyCollection<TData>> DequeueAsync(
            int count,
            CancellationToken cancellationToken)
        {
            var batch = new List<TData>();

            for (var i = 0; i < count; i++)
            {
                var file = await _filesReader.ReadAsync(cancellationToken);
                var fileName = Path.GetFileNameWithoutExtension(file);

                using (await FilesLock.ReadLockAsync(fileName, cancellationToken))
                {
                    byte[] dataBytes = await FileHelper
                        .ReadAllBytesAsync(file, cancellationToken);

                    TData data = Deserialize(dataBytes, fileName);

                    if (data != null)
                    {
                        batch.Add(data);
                        TryDelete(file);
                    }
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

        private async Task EnqueueAsync(
            TData data,
            CancellationToken cancellationToken)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var fileName = EncodeFileName(data);
            var fileFullName = Path.Combine(_storagePath, $"{fileName}.tmp");

            using (await FilesLock.WriteLockAsync(fileName, cancellationToken))
            {
                await FileHelper
                    .WriteAllBytesAsync(fileFullName, Serialize(data), cancellationToken);
            }

            await _filesWriter.WriteAsync(fileFullName, cancellationToken);
        }

        private void TryDelete(string fullName)
        {
            try
            {
                File.Delete(fullName);
            }
            catch (Exception)
            {
                // Don't crash could not be deleted.
            }
        }
    }
}
