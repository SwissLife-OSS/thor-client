using System;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Thor.Core.Abstractions;
using Thor.Extensions.HotChocolate;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// A bunch of convenient extensions methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ThorHotChocolateRequestExecutorBuilderExtensions
    {
        /// <summary>
        /// Adds <c>Thor HotChocolate Tracing</c> services to the service collection
        /// </summary>
        /// <param name="builder">A <see cref="IRequestExecutorBuilder"/> instance.</param>
        /// <returns>The provided <see cref="IRequestExecutorBuilder"/> instance.</returns>
        public static IRequestExecutorBuilder AddThorLogging(
            this IRequestExecutorBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder
                .AddDiagnosticEventListener(sp =>
                    new HotChocolateDiagnosticsListener(
                        sp.GetApplicationService<IRequestFormatter>()))
                .Services
                .AddSingleton<IProvidersDescriptor, HotChocolateProvidersDescriptor>()
                .TryAddSingleton<IRequestFormatter, DefaultRequestFormatter>();

            return builder;
        }
    }
}
