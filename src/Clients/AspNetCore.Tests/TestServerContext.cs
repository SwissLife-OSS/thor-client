using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Thor.AspNetCore.Tests
{
    public class TestServerContext
        : IDisposable
    {
        public TestServerContext()
        {
            Server = new TestServer(new WebHostBuilder().UseStartup<TestServerStartup>());
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