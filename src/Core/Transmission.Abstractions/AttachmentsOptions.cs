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
        public BufferOptions Buffer { get; set; } = new BufferOptions();

        /// <summary>
        /// Options for storage throttling.
        /// </summary>
        public StorageOptions Storage { get; set; } = new StorageOptions();
    }
}
