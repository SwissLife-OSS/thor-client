using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Thor.Core;

namespace Thor.GenericHost
{
    /// <summary>
    /// Provides base functionality of catching exeptions in a HostedService
    /// </summary>
    public abstract class HostedServiceBase : IHostedService
    {
        /// <inheritdoc cref="IHostedService"/>
        protected abstract Task OnStartAsync(CancellationToken cancellationToken);

        /// <inheritdoc cref="IHostedService"/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await OnStartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Application.UnhandledException(ex);
            }
        }

        /// <inheritdoc cref="IHostedService"/>
        protected abstract Task OnStopAsync(CancellationToken cancellationToken);

        /// <inheritdoc cref="IHostedService"/>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await OnStopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Application.UnhandledException(ex);
            }
        }
    }
}
