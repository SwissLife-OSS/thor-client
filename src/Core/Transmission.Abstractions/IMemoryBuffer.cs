using System.Collections.Generic;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A memory buffer for incoming data.
    /// </summary>
    public interface IMemoryBuffer<TData>
        where TData : class
    {
        /// <summary>
        /// Gets the count of items in buffer.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Dequeue data batch from the buffer.
        /// </summary>
        IReadOnlyCollection<TData> Dequeue(int count);

        /// <summary>
        /// Enqueue data object.
        /// </summary>
        void Enqueue(TData data);
    }
}
