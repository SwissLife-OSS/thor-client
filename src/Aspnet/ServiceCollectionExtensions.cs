using Microsoft.Extensions.DependencyInjection;

namespace Thor.Core.Aspnet
{
    /// <summary>
    /// A bunch of convenient extensions methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds configuration for tracing.
        /// </summary>
        /// <param name="collection">A service collection to add the tracing configuration to.</param>
        /// <returns>The provided service collection.</returns>
        public static IServiceCollection AddTracing(this IServiceCollection collection)
        {
            return collection
                .AddOptions()
                .Configure<TracingMiddlewareConfiguration>(c => { });
        }
    }
}