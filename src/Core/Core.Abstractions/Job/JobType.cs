namespace Thor.Core.Abstractions
{
    /// <summary>
    /// Defines jobs types
    /// </summary>
    public enum JobType
    {
        /// <summary>
        /// Attachment storage job
        /// </summary>
        AttachmentsStorage,

        /// <summary>
        /// Attachment sender job
        /// </summary>
        AttachmentsSender,

        /// <summary>
        /// Events storage job
        /// </summary>
        EventsStorage,

        /// <summary>
        /// Events sender job
        /// </summary>
        EventsSender,

        /// <summary>
        /// Events aggregator job
        /// </summary>
        EventsAggregator
    }
}
