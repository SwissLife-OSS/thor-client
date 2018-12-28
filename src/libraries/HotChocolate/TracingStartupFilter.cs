using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Thor.Core;
using Thor.Core.Session.Abstractions;

namespace Thor.HotChocolate
{
    /// <summary>
    /// A <see cref="IStartupFilter"/> to initialize tracing.
    /// </summary>
    public class TracingStartupFilter
        : IStartupFilter
    {
        private readonly IOptions<TracingConfiguration> _configurationAccessor;
        private readonly ITelemetrySession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="TracingStartupFilter"/> class.
        /// </summary>
        /// <param name="applicationLifetime">A application lifetime object instance.</param>
        /// <param name="configurationAccessor">A configuration accessor instance.</param>
        /// <param name="session">A telemetry event session.</param>
        public TracingStartupFilter(IApplicationLifetime applicationLifetime,
            IOptions<TracingConfiguration> configurationAccessor, ITelemetrySession session)
        {
            if (applicationLifetime == null)
            {
                throw new ArgumentNullException(nameof(applicationLifetime));
            }
            
            _configurationAccessor = configurationAccessor ??
                throw new ArgumentNullException(nameof(configurationAccessor));
            _session = session ?? throw new ArgumentNullException(nameof(session));

            Start();
            applicationLifetime.ApplicationStopping.Register(Stop);
        }

        private void Start()
        {
            Application.Start(_configurationAccessor.Value.ApplicationId);
        }

        private void Stop()
        {
            Application.Stop();
            // todo: implement Flush() to ensure all events are pushed before stopping the app
            _session?.Dispose();
        }

        /// <inheritdoc />
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder
                    .ApplicationServices
                    .GetService<DiagnosticsListenerInitializer>()
                    .Start();

                next(builder);
            };
        }
    }
}
