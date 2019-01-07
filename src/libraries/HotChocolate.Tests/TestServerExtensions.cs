using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace Thor.HotChocolate.Tests
{
    internal static class TestServerExtensions
    {
        public static Task<HttpResponseMessage> SendRequestAsync<TObject>(
            this TestServer testServer, TObject requestBody, string path = null)
        {
            var httpContent = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json");

            return testServer.CreateClient()
                .PostAsync(path ?? "/",
                    httpContent);
        }
    }
}
