using System;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// Jobs health report
    /// </summary>
    public class JobHealthCheck : IJobHealthCheck
    {
        private DateTime _attachmentsStorageRunAt;
        private DateTime _attachmentsSenderRunAt;
        private DateTime _eventsStorageRunAt;
        private DateTime _eventsSenderRunAt;
        private DateTime _eventsAggregatorRunAt;

        /// <inheritdoc cref="IJobHealthCheck"/>
        public void ReportAlive(JobType jobType)
        {
            switch (jobType)
            {
                case JobType.AttachmentsStorage:
                    _attachmentsStorageRunAt = DateTime.UtcNow;
                    return;
                case JobType.AttachmentsSender:
                    _attachmentsSenderRunAt = DateTime.UtcNow;
                    return;
                case JobType.EventsStorage:
                    _eventsStorageRunAt = DateTime.UtcNow;
                    return;
                case JobType.EventsSender:
                    _eventsSenderRunAt = DateTime.UtcNow;
                    return;
                case JobType.EventsAggregator:
                    _eventsAggregatorRunAt = DateTime.UtcNow;
                    return;

                default:
                    return;
            }
        }

        /// <inheritdoc cref="IJobHealthCheck"/>
        public JobsHealthReport GetReport()
        {
            return new JobsHealthReport
            {
                AttachmentsSenderRunAt = _attachmentsSenderRunAt,
                AttachmentsStorageRunAt = _attachmentsStorageRunAt,
                EventsSenderRunAt = _eventsSenderRunAt,
                EventsStorageRunAt = _eventsStorageRunAt,
                EventsAggregatorRunAt = _eventsAggregatorRunAt
            };
        }
    }
}
