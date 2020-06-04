using System;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Abstractions
{
    internal class Job : IDisposable
    {
        internal static Job Start(
            Func<Task> action,
            JobType jobType,
            IJobHealthCheck jobHealthCheck,
            CancellationToken cancellationToken)
        {
            return Start(action, () => false, jobType, jobHealthCheck, cancellationToken);
        }

        internal static Job Start(
            Func<Task> action,
            Func<bool> spinWhen,
            JobType jobType,
            IJobHealthCheck jobHealthCheck,
            CancellationToken cancellationToken)
        {
            var job = new Job(jobType, jobHealthCheck, cancellationToken);

            Task.Factory.StartNew(
                () => job.Start(action, spinWhen),
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return job;
        }

        private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(50);
        private readonly JobType _jobType;
        private readonly IJobHealthCheck _jobHealthCheck;
        private readonly CancellationToken _cancellationToken;
        private readonly ManualResetEventSlim _sync;

        private Job(JobType jobType, IJobHealthCheck jobHealthCheck, CancellationToken cancellationToken)
        {
            _jobType = jobType;
            _jobHealthCheck = jobHealthCheck;
            _cancellationToken = cancellationToken;
            Stopped = false;
            _sync = new ManualResetEventSlim();
        }

        private async Task Start(Func<Task> action, Func<bool> spinWhen)
        {   
            _cancellationToken.ThrowIfCancellationRequested();

            while (!_cancellationToken.IsCancellationRequested)
            {
                _jobHealthCheck.ReportAlive(_jobType);
                await action();

                if (!_cancellationToken.IsCancellationRequested && spinWhen())
                {
                    await Task
                        .Delay(Delay, _cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            Stopped = true;
            _sync.Set();
            _cancellationToken.ThrowIfCancellationRequested();
        }

        internal bool Stopped { get; private set; }
        internal WaitHandle WaitHandle => _sync.WaitHandle;

        public void Dispose()
        {
            _sync?.Dispose();
        }
    }
}
