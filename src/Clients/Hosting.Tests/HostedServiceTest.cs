using System;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Extensions.Hosting.Tests
{
    internal class HostedServiceTest : HostedServiceBase
    {
        private readonly TaskCompletionSource<bool> _start =
            new TaskCompletionSource<bool>();

        private readonly TaskCompletionSource<bool> _stop =
            new TaskCompletionSource<bool>();

        protected override Task OnStartAsync(
            CancellationToken cancellationToken)
        {
            if (StartShouldFail)
            {
                throw new InvalidOperationException();
            }

            _start.SetResult(true);
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync(
            CancellationToken cancellationToken)
        {
            if (StopShouldFail)
            {
                throw new InvalidOperationException();
            }

            _stop.SetResult(true);
            return Task.CompletedTask;
        }

        internal Task<bool> WaitStart => _start.Task;
        internal bool StartShouldFail { get; set; }

        internal Task<bool> WaitStop => _stop.Task;
        internal bool StopShouldFail { get; set; }
    }
}
