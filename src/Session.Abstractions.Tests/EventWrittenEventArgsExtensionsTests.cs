using System;
using System.Diagnostics.Tracing;
using System.Linq;
using Thor.Core.Abstractions;
using Xunit;

namespace Thor.Core.Session.Abstractions.Tests
{
    public class EventWrittenEventArgsExtensionsTests
    {
        [Fact(DisplayName = "Map: Should throw an argument null exception for source")]
        public void Map_SourceNull()
        {
            // arrange
            EventWrittenEventArgs @event = null;
            string sessionName = "session 456";

            // act
            Action verify = () => @event.Map(sessionName);

            // assert
            Assert.Throws<ArgumentNullException>("source", verify);
        }

        [Fact(DisplayName = "Map: Should throw an argument null exception for sessionName")]
        public void Map_SessionNameNull()
        {
            TestEventSource.Log.Listen(listener =>
            {
                // arrange
                TestEventSource.Log.DoSomething();
                string sessionName = null;
                EventWrittenEventArgs @event = listener
                    .OrderedEvents
                    .FirstOrDefault();

                // act
                Action verify = () => @event.Map(sessionName);

                // assert
                Assert.Throws<ArgumentNullException>("sessionName", verify);
            });
        }

        [Fact(DisplayName = "Map: Should return an event")]
        public void Map_Success()
        {
            TestEventSource.Log.Listen(listener =>
            {
                // arrange
                TestEventSource.Log.DoSomething();
                string sessionName = "8809";
                EventWrittenEventArgs @event = listener
                    .OrderedEvents
                    .FirstOrDefault();

                // act
                TelemetryEvent result = @event.Map(sessionName);

                // assert
                Assert.Equal(Guid.Empty, result.ActivityId);
                Assert.Equal(0, result.ApplicationId);
                Assert.Null(result.AttachmentId);
                Assert.Equal(0, result.Channel);
                Assert.Equal(0, result.EnvironmentId);
                Assert.Equal(1, result.Id);
                Assert.Equal(EventLevel.Informational, result.Level);
                Assert.Equal("Test message 555", result.Message);
                Assert.Equal("DoSomething", result.Name);
                Assert.Equal(0, result.OpcodeId);
                Assert.Equal("Info", result.OpcodeName);
                Assert.Null(result.Payload);
                Assert.Equal(0, result.ProcessId);
                Assert.Null(result.ProcessName);
                Assert.Equal("ChilliCream-Test", result.ProviderName);
                Assert.Equal(Guid.Empty, result.RelatedActivityId);
                Assert.Equal("8809", result.SessionName);
                Assert.Equal(0, result.ThreadId);
                Assert.NotEqual(0, result.Timestamp);
                Assert.Null(result.UserId);
                Assert.Equal(1, result.Version);
            });
        }
    }
}