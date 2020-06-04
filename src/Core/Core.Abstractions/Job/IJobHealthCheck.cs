namespace Thor.Core.Abstractions
{
    /// <summary>
    /// Job health check
    /// </summary>
    public interface IJobHealthCheck
    {
        /// <summary>
        /// Report job last run
        /// </summary>
        void ReportAlive(JobType jobType);

        /// <summary>
        /// Get all jobs report
        /// </summary>
        JobsHealthReport GetReport();
    }
}
