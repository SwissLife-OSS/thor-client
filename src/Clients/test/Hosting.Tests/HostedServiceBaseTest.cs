using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thor.Core;
using Thor.Core.Abstractions;
using Thor.Core.Session.Abstractions;
using Xunit;

namespace Thor.Extensions.Hosting.Tests
{
    public class HostedServiceBaseTest
    {
        [Fact]
        public async Task HostedServiceBase_WhenStartAndStop_ShouldPass()
        {
            // arrange
            var hostedService = new HostedServiceTest();
            var host = HostHelper.Build(hostedService);

            // act
            var hostStart = host.StartAsync();
            var start = await hostedService.WaitStart;
            var hostStop = host.StopAsync();
            var stop = await hostedService.WaitStop;

            // assert
            Assert.True(start);
            Assert.True(stop);
        }

        [Fact]
        public async Task HostedServiceBase_WhenStartThrowsAndStopExecuted_ShouldPassAndFireEvent()
        {
            // arrange
            var hostedService = new HostedServiceTest {StartShouldFail = true};
            var host = HostHelper.Build(hostedService);
            List<TelemetryEvent> events = new List<TelemetryEvent>();

            // act
            ApplicationEventSource.Log.Listen(listener =>
            {
                var hostStart = host.StartAsync();
                Task.Delay(100).GetAwaiter().GetResult();

                events.AddRange(listener
                    .OrderedEvents
                    .Select(e => EventWrittenEventArgsExtensions.Map(e, "test"))
                    .ToArray());
            });
            var hostStop = host.StopAsync();
            var stop = await hostedService.WaitStop;

            // assert
            Assert.NotNull(events
                .FirstOrDefault(e => e.Message == "Unhandled exception occurred."));
            Assert.True(stop);
        }

        [Fact]
        public async Task HostedServiceBase_WhenStartExecutedAndStopThrows_ShouldPassAndFireEvent()
        {
            // arrange
            var hostedService = new HostedServiceTest { StopShouldFail = true };
            var host = HostHelper.Build(hostedService);
            List<TelemetryEvent> events = new List<TelemetryEvent>();

            // act
            var hostStart = host.StartAsync();
            var start = await hostedService.WaitStart;
            ApplicationEventSource.Log.Listen(listener =>
            {
                var hostStop = host.StopAsync();
                Task.Delay(100).GetAwaiter().GetResult();

                events.AddRange(listener
                    .OrderedEvents
                    .Select(e => e.Map("test"))
                    .ToArray());
            });

            // assert
            Assert.True(start);
            Assert.NotNull(events
                .FirstOrDefault(e => e.Message == "Unhandled exception occurred."));
        }

        [Fact]
        public async Task HostedServiceBase_WhenStartAndStopThrows_ShouldFireEvents()
        {
            // arrange
            var hostedService = new HostedServiceTest { StopShouldFail = true };
            var host = HostHelper.Build(hostedService);
            List<TelemetryEvent> events = new List<TelemetryEvent>();

            // act
            var hostStart = host.StartAsync();
            var start = await hostedService.WaitStart;
            ApplicationEventSource.Log.Listen(listener =>
            {
                var hostStop = host.StopAsync();
                Task.Delay(100).GetAwaiter().GetResult();

                events.AddRange(listener
                    .OrderedEvents
                    .Select(e => e.Map("test"))
                    .ToArray());
            });

            // assert
            Assert.True(start);
            Assert.NotNull(events
                .FirstOrDefault(e => e.Message == "Unhandled exception occurred."));
        }
    }
}