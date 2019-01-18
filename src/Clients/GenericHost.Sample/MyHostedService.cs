using System.Threading;
using System.Threading.Tasks;
using Thor.GenericHost;

namespace GenericHost.Sample
{
    internal class MyHostedService : HostedServiceBase
    {
        protected override Task OnStartAsync(CancellationToken cancellationToken)
        {
            // Start some work
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync(CancellationToken cancellationToken)
        {
            // Stop the work
            return Task.CompletedTask;
        }
    }
}
