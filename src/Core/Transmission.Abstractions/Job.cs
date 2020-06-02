using System;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    internal class Job : IDisposable
    {
        internal static Job Start(
            Func<Task> action,
            CancellationToken cancellationToken)
        {
            return Start(action, () => false, cancellationToken);
        }

        internal static Job Start(
            Func<Task> action,
            Func<bool> spinWhen,
            CancellationToken cancellationToken)
        {
            var job = new Job(cancellationToken);

            Task.Factory.StartNew(
                () => job.Start(action, spinWhen),
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return job;
        }

        private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(50);
        private readonly CancellationToken _cancellationToken;
        private readonly ManualResetEventSlim _sync;

        private Job(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            Stopped = false;
            _sync = new ManualResetEventSlim();
        }

        private async Task Start(Func<Task> action, Func<bool> spinWhen)
        {   
            _cancellationToken.ThrowIfCancellationRequested();

            while (!_cancellationToken.IsCancellationRequested)
            {
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
