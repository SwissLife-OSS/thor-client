using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Thor.Core;
using Thor.Core.Session.Abstractions;
using Thor.Extensions.Http;
using Xunit;

namespace Thor.Hosting.AspNetCore.Tests
{
    public class HostingDiagnosticListenerTests
    {
        [Fact]
        public async Task CreateRequest_WhenSkipRequestIsEmpty_TriggerEvents()
        {
            var configuration = new Dictionary<string, string>
            {
                {"Tracing:ApplicationId", "999"},
                {"Tracing:SkipRequestFilter", ""}
            };

            await RequestActivityEventSource.Log.ListenAsync(async listener =>
            {
                using var context = new TestServerContext(configuration);
                await context.Client.GetAsync("/get");

                Assert.Collection(listener.OrderedEvents,
                    startEvent => Assert.Equal(EventOpcode.Start, startEvent.Opcode),
                    stopEvent => Assert.Equal(EventOpcode.Stop, stopEvent.Opcode));
            });
        }

        [Fact]
        public async Task CreateRequest_WhenSkipRequestHasValue_NoEventsAreTriggered()
        {
            var configuration = new Dictionary<string, string>
            {
                {"Tracing:ApplicationId", "999"},
                {"Tracing:SkipRequestFilter", "_health"}
            };

            await RequestActivityEventSource.Log.ListenAsync(async listener =>
            {
                using var context = new TestServerContext(configuration);
                await context.Client.GetAsync("/_health/readyness");

                Assert.Empty(listener.OrderedEvents);
            });
        }

        [Fact]
        public async Task CreateRequest_WhenSkipRequestHasValueAndThrowsException_TriggerEventS()
        {
            var configuration = new Dictionary<string, string>
            {
                {"Tracing:ApplicationId", "999"},
                {"Tracing:SkipRequestFilter", "_health"}
            };

            await ApplicationEventSource.Log.ListenAsync(async listener =>
            {
                using var context = new TestServerContext(configuration);
                await context.Client.GetAsync("/_health/liveness");

                Assert.Collection(listener.OrderedEvents,
                    errorEvent => Assert.Equal(EventLevel.Error, errorEvent.Level));
            });
        }
    }
}
