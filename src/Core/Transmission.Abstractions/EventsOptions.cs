namespace Thor.Core.Transmission.Abstractions
{
    /// <summary>
    /// Options for events processing
    /// </summary>
    public class EventsOptions
    {
        /// <summary>
        /// Options for enqueue buffer
        /// </summary>
        public BufferOptions Buffer { get; set; } = new BufferOptions();
    }
}
