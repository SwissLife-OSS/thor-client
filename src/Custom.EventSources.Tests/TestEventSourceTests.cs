// forked from https://github.com/ChilliCream/thor-core

using System.Diagnostics.Tracing;
using System.Linq;
using Thor.Core.Session.Abstractions;
using Xunit;

namespace Custom.EventSources.Tests
{
    public class TestEventSourceTests
    {
        [Fact(DisplayName = "Analyze: Inspects TestEventSource schema")]
        public void Analyze()
        {
            // arrange
            var analyzer = new Thor.Analyzer.EventSourceAnalyzer();

            // act
            Thor.Analyzer.Report report = analyzer.Inspect(
                TestEventSource.Log as EventSource);

            // assert
            Assert.False(report.HasErrors);
        }

        #region RunProcess

        [Fact(DisplayName = "RunProcess: Should not write anything to the log stream")]
        public void RunProcessDisabled()
        {
            using (var listener = new ProbeEventListener())
            {
                // act
                TestEventSource.Log.RunProcess(333);

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "RunProcess: Should write one message to the log stream")]
        public void RunProcessEnabled()
        {
            (TestEventSource.Log as EventSource).Listen((listener) =>
            {
                // arrange
                string expectedMessage = "Run process \"{0}\"";

                // act
                TestEventSource.Log.RunProcess(444);

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Equal(3, firstItem.Payload.Count);
                Assert.Equal(1, firstItem.EventId);
                Assert.Equal(EventLevel.Verbose, firstItem.Level);
                Assert.Equal(444, firstItem.Payload[2]);
            });
        }

        #endregion
    }
}
