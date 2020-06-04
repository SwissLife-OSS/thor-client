using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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
        private readonly ChannelReader<string> _dequeueFiles;
        private readonly ChannelWriter<string> _enqueueFiles;

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
            _dequeueFiles = files.Reader;
            _enqueueFiles = files.Writer;

            IEnumerable<FileInfo> unprocessedFiles = Directory
                .CreateDirectory(_storagePath)
                .EnumerateFiles("*.tmp", SearchOption.TopDirectoryOnly);

            foreach (FileInfo fileInfo in unprocessedFiles)
            {
                _enqueueFiles.TryWrite(fileInfo.FullName);
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
        public async IAsyncEnumerable<TData> DequeueAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (await _dequeueFiles.WaitToReadAsync(cancellationToken))
            {
                while (_dequeueFiles.TryRead(out var fileFullName))
                {
                    var fileName = Path.GetFileNameWithoutExtension(fileFullName);

                    byte[] dataBytes = await FileHelper
                        .ReadAllBytesAsync(fileFullName, cancellationToken);

                    TData data = Deserialize(dataBytes, fileName);

                    if (data != null)
                    {
                        yield return data;
                        TryDelete(fileFullName);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task EnqueueAsync(
            IAsyncEnumerable<TData> batch,
            CancellationToken cancellationToken)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            await foreach (TData data in batch.WithCancellation(cancellationToken))
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

            await FileHelper
                .WriteAllBytesAsync(fileFullName, Serialize(data), cancellationToken);

            await _enqueueFiles.WriteAsync(fileFullName, cancellationToken);
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
