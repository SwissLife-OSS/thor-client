using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Thor.Core;
using Thor.Core.Session.Abstractions;

namespace Thor.GenericHost
{
    internal class HostTelemetryInitializer
    {
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly ITelemetrySession _session;
        private readonly IOptions<TracingConfiguration> _configurationAccessor;

        public HostTelemetryInitializer(
            IApplicationLifetime applicationLifetime,
            ITelemetrySession session,
            IOptions<TracingConfiguration> configurationAccessor)
        {
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _configurationAccessor = configurationAccessor ?? throw new ArgumentNullException(nameof(configurationAccessor));

            RegisterForUnhandledExceptions();
        }

        private void RegisterForUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Application.UnhandledException(args.ExceptionObject as Exception);
                _session?.Dispose();
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                Application.UnhandledException(args.Exception);
                _session?.Dispose();
            };
        }

        public void Initialize()
        {
            _applicationLifetime
                .ApplicationStarted
                .Register(Start);

            _applicationLifetime
                .ApplicationStopped
                .Register(Stop);
        }

        private void Start()
        {
            Application.Start(_configurationAccessor.Value.ApplicationId);
        }

        private void Stop()
        {
            Application.Stop();
            _session?.Dispose();
        }
    }
}
