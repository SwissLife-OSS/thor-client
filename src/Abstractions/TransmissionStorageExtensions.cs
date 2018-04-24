using System;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// A bunch of convenient <see cref="ITransmissionStorage{TData}"/> extension methods.
    /// </summary>
    public static class TransmissionStorageExtensions
    {
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

        /// <summary>
        /// Tries to dequeue a telemetry data batch from the storage.
        /// </summary>
        /// <param name="storage">A transmission storage instance.</param>
        /// <param name="batch">A telemetry data batch.</param>
        /// <returns>A value indicating whether a telemetry data batch could be returned.</returns>
        public static Task<bool> TryDequeueAsync<TData>(this ITransmissionStorage<TData> storage,
            out TData[] batch)
                where TData : class
        {
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }

            return storage.TryDequeueAsync(out batch, CancellationToken.None);
        }
    }
}