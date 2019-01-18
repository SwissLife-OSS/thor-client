using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Thor.GenericHost;

namespace GenericHost.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> configuration = new Dictionary<string, string>
            {
                {"Tracing:ApplicationId", "999"},
                {"Tracing:Level", "Verbose"},
                {"Tracing:BlobStorage:ConnectionString", "***"},
                {"Tracing:BlobStorage:AttachmentContainerName", "demo"},
                {"Tracing:EventHub:ConnectionString", "***"}
            };

            new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                    builder.AddInMemoryCollection(configuration))
                .ConfigureServices(services =>
                {
                    services.AddHostedService<MyHostedService>();
                    services.AddHostedService<MyBackgroundService>();
                })
                .RunWithTracing();
        }
    }
}
