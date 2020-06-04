using System;
using Microsoft.Extensions.Hosting;
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
        private readonly TracingConfiguration _options;

        public HostTelemetryInitializer(
            IApplicationLifetime applicationLifetime,
            ITelemetrySession session,
            IAttachmentTransmissionInitializer initializer,
            TracingConfiguration options)
        {
            _applicationLifetime = applicationLifetime ??
                throw new ArgumentNullException(nameof(applicationLifetime));
            _session = session ??
                throw new ArgumentNullException(nameof(session));
            _initializer = initializer
                ?? throw new ArgumentNullException(nameof(initializer));
            _options = options ??
                throw new ArgumentNullException(nameof(options));

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
            Application.Start(_options.ApplicationId);
        }

        private void Stop()
        {
            Application.Stop();
            _session.Dispose();
        }
    }
}
