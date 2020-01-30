namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// A concrete configuration for <c>Azure EventHub</c> transmission.
    /// </summary>
    public class EventHubConfiguration
    {
        /// <summary>
        /// Gets or sets the <c>Azure EventHub</c> connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the <c>Azure EventHub</c> connection type.
        /// </summary>
        public string TransportType { get; set; }
    }
}