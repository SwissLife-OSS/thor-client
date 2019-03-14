using HotChocolate.Execution;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Thor.Extensions.HotChocolate
{
    internal static class HotChocolateExtensions
    {
        internal static HttpContext GetHttpContext(
            this IQueryContext queryContext)
        {
            return queryContext
                .Services
                .GetService<HttpContext>();
        }

        internal static HttpContext GetHttpContext(
            this IResolverContext resolverContext)
        {
            return resolverContext.Service<HttpContext>();
        }
    }
}