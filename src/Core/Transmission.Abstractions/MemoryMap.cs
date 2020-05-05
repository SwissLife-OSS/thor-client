using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    internal struct MemoryMap<T> where T : class
    {
        private readonly string _name;
        private readonly string _filePath;
        private readonly string _mutexName;

        public MemoryMap(string name, string filePath)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _name = name;
            _mutexName = $"{_name}-Mutex";
            _filePath = filePath;
        }

        internal Task CreateAsync(
            T data,
            Func<T, byte[]> serialize,
            CancellationToken cancellationToken)
        {
            Mutex mutex = GetMutex();

            try
            {
                byte[] dataBytes = serialize(data);

                using var file = MemoryMappedFile
                    .CreateFromFile(_filePath, FileMode.CreateNew, _name, dataBytes.Length);

                using MemoryMappedViewStream viewStream = file.CreateViewStream();

                return viewStream
                    .WriteAsync(dataBytes, 0, dataBytes.Length, cancellationToken);
            }
            catch
            {
                // Don't need any exception if something happened here
                return Task.CompletedTask;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public async Task<T> LoadAsync(
            Func<byte[], string, T> deserialize,
            CancellationToken cancellationToken)
        {
            Mutex mutex = GetMutex();

            try
            {
                using var file = MemoryMappedFile
                    .CreateFromFile(_filePath, FileMode.Open, _name);

                using MemoryMappedViewStream viewStream = file.CreateViewStream();

                return deserialize(await ReadAsync(viewStream, cancellationToken), _name);
            }
            catch
            {
                // Don't need any exception if file is not accessible or was not found
                return default;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private Mutex GetMutex()
        {
            if (Mutex.TryOpenExisting(_mutexName, out Mutex mutex))
            {
                mutex.WaitOne();
            }
            else
            {
                mutex = new Mutex(true, _mutexName, out var mutexCreated);
                if (!mutexCreated)
                {
                    mutex.WaitOne();
                }
            }

            return mutex;
        }

        private async Task<byte[]> ReadAsync(
            MemoryMappedViewStream viewStream,
            CancellationToken cancellationToken)
        {
            const int bufferSize = 4096;

            using var memoryStream = new MemoryStream();
            var buffer = new byte[bufferSize];
            int count;

            while ((count = await viewStream
                .ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
            {
                await memoryStream
                    .WriteAsync(buffer, 0, count, cancellationToken);
            }

            return memoryStream.ToArray();
        }
    }
}
