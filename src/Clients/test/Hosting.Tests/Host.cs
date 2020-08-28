using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Thor.Extensions.Hosting.Tests
{
    internal static class HostHelper
    {
        internal static IHost Build<T>(T service)
            where T : class, IHostedService
        {
            return new HostBuilder()
                .ConfigureServices(services =>
                    services.AddTransient<IHostedService, T>(
                        provider => service))
                .Build();
        }
    }
}