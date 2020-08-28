using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Thor.Hosting.AspNetCore.Tests
{
    public class TestServerContext
        : IDisposable
    {
        public TestServerContext(IEnumerable<KeyValuePair<string, string>> configuration)
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(c => c.AddInMemoryCollection(configuration))
                .UseStartup<TestServerStartup>();

            Server = new TestServer(webHostBuilder);
            Client = Server.CreateClient();
        }

        public HttpClient Client { get; }

        public TestServer Server { get; }

        public void Dispose()
        {
            Client?.Dispose();
            Server?.Dispose();
        }
    }
}