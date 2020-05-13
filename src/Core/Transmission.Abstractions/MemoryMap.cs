using System;
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

                return FileHelper
                    .WriteAllBytesAsync(_filePath, dataBytes, cancellationToken);
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
                byte[] dataBytes = await FileHelper
                    .ReadAllBytesAsync(_filePath, cancellationToken);

                return deserialize(dataBytes, _name);
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
    }
}
