using System.Threading;
using System.Threading.Tasks;
using Thor.GenericHost;

namespace GenericHost.Sample
{
    internal class MyBackgroundService : BackgroundServiceBase
    {
        protected override Task OnExecuteAsync(CancellationToken stoppingToken)
        {
            // Do some work
            return Task.CompletedTask;
        }
    }
}
