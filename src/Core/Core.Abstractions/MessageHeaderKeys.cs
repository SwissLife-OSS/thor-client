namespace Thor.Core.Abstractions
{
    /// <summary>
    /// A bunch of common keys for all kind of message headers -- HTTP, ServiceBus to name a few.
    /// </summary>
    public static class MessageHeaderKeys
    {
        /// <summary>
        /// Gets the header key for the activity identifier.
        /// </summary>
        public const string ActivityId = "Thor-ActivityId";
    }
}