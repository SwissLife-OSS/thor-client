using System;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thor.Core.Abstractions;

namespace Thor.Extensions.HotChocolate
{
    /// <summary>
    /// A bunch of convenient extensions methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <c>Thor HotChocolate Tracing</c> services to the service collection
        /// </summary>
        /// <param name="builder">A <see cref="IQueryExecutionBuilder"/> instance.</param>
        /// <returns>The provided <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddHotChocolateTracing(
            this IServiceCollection builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder
                .AddDiagnosticObserver<HotChocolateDiagnosticsListener>()
                .AddSingleton<IProvidersDescriptor, HotChocolateProvidersDescriptor>()
                .TryAddSingleton<IRequestFormatter, DefaultRequestFormatter>();

            return builder;
        }
    }
}
