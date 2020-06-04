using System.Collections.Generic;
using System.Threading;

namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// A memory buffer for incoming data.
    /// </summary>
    public interface IMemoryBuffer<TData>
        where TData : class
    {
        /// <summary>
        /// Dequeue data batch from the buffer.
        /// </summary>
        IAsyncEnumerable<TData> Dequeue(CancellationToken cancellationToken);

        /// <summary>
        /// Enqueue data object.
        /// </summary>
        void Enqueue(TData data);
    }
}
