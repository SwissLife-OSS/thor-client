using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Thor.Extensions.HotChocolate
{
    internal static class HotChocolateExtensions
    {
        internal static HttpContext GetHttpContext(
            this IReadOnlyQueryRequest queryRequest)
        {
            return queryRequest
                .Services
                .GetService<HttpContext>();
        }
    }
}