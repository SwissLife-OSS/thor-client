using System;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// Options for buffering used for both Events and Attachments
    /// </summary>
    public class BufferOptions
    {
        /// <summary>
        /// Buffer size limit.
        /// Default 1000 items.
        /// </summary>
        public int Size { get; set; } = 1000;

        /// <summary>
        /// Try to enqueue timeout if buffer is full. This will block the calling thread.
        /// If set to -1 milliseconds will block until buffer will have free space.
        /// Default 1 second.
        /// </summary>
        public TimeSpan EnqueueTimeout { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Size of the dequeue batch.
        /// </summary>
        public int DequeueBatchSize { get; set; } = 100;
    }
}
