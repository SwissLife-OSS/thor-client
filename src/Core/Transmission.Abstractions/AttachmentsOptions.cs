namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// Options for attachments processing
    /// </summary>
    public class AttachmentsOptions
    {
        /// <summary>
        /// Options for enqueue buffer
        /// </summary>
        public BufferOptions Buffer { get; set; }

        /// <summary>
        /// Size of the dequeue batch from buffer.
        /// </summary>
        public int BufferDequeueBatchSize { get; set; } = 100;

        /// <summary>
        /// Size of the dequeue batch from storage.
        /// </summary>
        public int StorageDequeueBatchSize { get; set; } = 100;
    }
}
