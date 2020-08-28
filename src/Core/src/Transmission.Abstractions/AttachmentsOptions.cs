namespace Thor.Core.Transmission.Abstractions
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
    }
}
