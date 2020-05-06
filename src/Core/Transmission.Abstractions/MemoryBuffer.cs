using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A memory transmission buffer
    /// </summary>
    public class MemoryBuffer<TData>
        : IMemoryBuffer<TData>
        where TData : class
    {
        private readonly BufferOptions _options;
        private readonly BlockingCollection<TData> _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBuffer{TData}"/> class.
        /// </summary>
        /// <param name="options"></param>
        public MemoryBuffer(BufferOptions options)
        {
            _options = options;
            _buffer = new BlockingCollection<TData>(_options.Size);
        }

        /// <inheritdoc />
        public int Count => _buffer.Count;

        /// <inheritdoc />
        public void Enqueue(TData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _buffer.TryAdd(data, _options.EnqueueTimeout);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<TData> Dequeue(int count)
        {
            var batch = new List<TData>(count);

            for (var i = 0; i < count; i++)
            {
                if (_buffer.TryTake(out TData data))
                {
                    batch.Add(data);
                }
            }

            return batch;
        }
    }
}
