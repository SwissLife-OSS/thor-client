using System;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// Jobs health report
    /// </summary>
    public class JobsHealthReport
    {
        /// <summary>
        /// Attachments storage last run
        /// </summary>
        public DateTime AttachmentsStorageRunAt { get; set; }

        /// <summary>
        /// Attachments sender last run
        /// </summary>
        public DateTime AttachmentsSenderRunAt { get; set; }

        /// <summary>
        /// Events storage last run
        /// </summary>
        public DateTime EventsStorageRunAt { get; set; }

        /// <summary>
        /// Events sender last run
        /// </summary>
        public DateTime EventsSenderRunAt { get; set; }

        /// <summary>
        /// Events aggregator last run
        /// </summary>
        public DateTime EventsAggregatorRunAt { get; set; }
    }
}
