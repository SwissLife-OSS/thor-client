using System;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thor.Core.Abstractions;

namespace Thor.Extensions.HotChocolate
{
    /// <summary>
    /// A bunch of convenient extensions methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class QueryExecutionBuilderExtensions
    {
        /// <summary>
        /// Adds <c>Thor HotChocolate Tracing</c> services to the service collection
        /// </summary>
        /// <param name="builder">A <see cref="IQueryExecutionBuilder"/> instance.</param>
        /// <param name="configuration">A <see cref="IConfiguration"/> instance.</param>
        /// <returns>The provided <see cref="IServiceCollection"/> instance.</returns>
        public static IQueryExecutionBuilder AddHotCocolateTracing(
            this IQueryExecutionBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder
                .AddDiagnosticObserver<HotChocolateDiagnosticsListener>();
        }
    }
}