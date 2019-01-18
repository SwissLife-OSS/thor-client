using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Thor.Core.Session.Abstractions;

namespace Thor.HotChocolate.Tests
{
    public class TestStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(
            Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder
                    .ApplicationServices
                    .GetService<ITelemetrySession>()
                    .Attach(ProbeTransmitter.Instance);

                next(builder);
            };
        }
    }
}