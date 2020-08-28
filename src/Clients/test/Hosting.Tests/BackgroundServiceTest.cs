using System;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Extensions.Hosting.Tests
{
    internal class BackgroundServiceTest : BackgroundServiceBase
    {
        private readonly TaskCompletionSource<bool> _execution =
            new TaskCompletionSource<bool>();

        protected override Task OnExecuteAsync(
            CancellationToken stoppingToken)
        {
            if (ShouldFail)
            {
                throw new InvalidOperationException();
            }

            _execution.SetResult(true);
            return Task.CompletedTask;
        }

        internal Task<bool> WaitExecution => _execution.Task;
        internal bool ShouldFail { get; set; }
    }
}
