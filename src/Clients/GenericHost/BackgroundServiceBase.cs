using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Thor.Core;

namespace Thor.Hosting.GenericHost
{
    /// <summary>
    /// Provides base functionality of catching exeptions in a BackgroundService
    /// </summary>
    public abstract class BackgroundServiceBase : BackgroundService
    {
        /// <inheritdoc cref="BackgroundService"/>
        protected abstract Task OnExecuteAsync(
            CancellationToken stoppingToken);

        /// <inheritdoc cref="BackgroundService"/>
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            try
            {
                await OnExecuteAsync(stoppingToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Application.UnhandledException(ex);
                throw;
            }
        }
    }
}