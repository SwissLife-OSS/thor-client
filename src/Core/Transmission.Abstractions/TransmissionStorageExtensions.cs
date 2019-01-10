using System;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A bunch of convenient <see cref="ITransmissionStorage{TData}"/> extension methods.
    /// </summary>
    public static class TransmissionStorageExtensions
    {
        /// <summary>
        /// Dequeues a telemetry data batch from the storage.
        /// </summary>
        /// <param name="storage">A transmission storage instance.</param>
        /// <returns>A telemetry data batch.</returns>
        public static Task<TData[]> DequeueAsync<TData>(this ITransmissionStorage<TData> storage)
            where TData : class
        {
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }

            return storage.DequeueAsync(CancellationToken.None);
        }

        /// <summary>
        /// Enqueues a telemetry data batch to a <c>short-term</c> storage.
        /// </summary>
        /// <param name="storage">A transmission storage instance.</param>
        /// <param name="batch">A telemetry data batch.</param>
        public static Task EnqueueAsync<TData>(this ITransmissionStorage<TData> storage, TData[] batch)
            where TData : class
        {
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }

            return storage.EnqueueAsync(batch, CancellationToken.None);
        }
    }
}