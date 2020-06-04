namespace Thor.Core.Transmission.Abstractions
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
    }
}
