using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Thor.Core;
using Thor.Core.Session.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Hosting.GenericHost
{
    internal class HostTelemetryInitializer
    {
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly ITelemetrySession _session;
        private readonly IAttachmentTransmissionInitializer _initializer;
        private readonly IOptions<TracingConfiguration> _configurationAccessor;

        public HostTelemetryInitializer(
            IApplicationLifetime applicationLifetime,
            ITelemetrySession session,
            IAttachmentTransmissionInitializer initializer,
            IOptions<TracingConfiguration> configurationAccessor)
        {
            _applicationLifetime = applicationLifetime ??
                throw new ArgumentNullException(nameof(applicationLifetime));
            _session = session ??
                throw new ArgumentNullException(nameof(session));
            _initializer = initializer
                ?? throw new ArgumentNullException(nameof(initializer));
            _configurationAccessor = configurationAccessor ??
                throw new ArgumentNullException(nameof(configurationAccessor));

            RegisterForUnhandledExceptions();
        }

        private void RegisterForUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Application.UnhandledException(args.ExceptionObject as Exception);
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
            _initializer.Initialize();
            Application.Start(_configurationAccessor.Value.ApplicationId);
        }

        private void Stop()
        {
            Application.Stop();
            _session.Dispose();
        }
    }
}
