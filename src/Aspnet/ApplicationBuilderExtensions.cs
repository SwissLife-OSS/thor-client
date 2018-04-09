using System;
using Microsoft.AspNetCore.Builder;

namespace Thor.Core.Aspnet
{
    /// <summary>
    /// A bunch of convenient extensions methods for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the tracing middleware at the last position.
        /// </summary>
        /// <param name="builder">A <see cref="IApplicationBuilder"/> instance.</param>
        public static void RunTracing(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.UseTracing();
        }

        /// <summary>
        /// Adds the tracing middleware.
        /// </summary>
        /// <param name="builder">A <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The provided <see cref="IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder UseTracing(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<TracingMiddleware>();
        }
    }
}