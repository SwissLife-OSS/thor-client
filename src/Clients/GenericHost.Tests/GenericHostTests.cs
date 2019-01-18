using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Thor.GenericHost.Tests
{
    public class GenericHostTests
    {
        public void Sample_GenericHost()
        {
            Dictionary<string, string> configuration = new Dictionary<string, string>
            {
                {"Tracing:ApplicationId", "999"},
                {"Tracing:Level", "Warning"},
                {"Tracing:BlobStorage:ConnectionString", ""},
                {"Tracing:EventHub:ConnectionString", ""}
            };

            new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                        builder.AddInMemoryCollection(configuration))
                .RunWithTracing();
        }
    }
}
