using System.Threading;
using System.Threading.Tasks;

namespace Thor.Extensions.Hosting.Tests
{
    internal class HostedServiceTest : HostedServiceBase
    {
        protected override Task OnStartAsync(
            CancellationToken cancellationToken)
        {
            // Start some work
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync(
            CancellationToken cancellationToken)
        {
            // Stop the work
            return Task.CompletedTask;
        }
    }
}
