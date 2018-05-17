using System.Diagnostics.Tracing;
using System.Linq;
using Thor.Core.Session.Abstractions;
using Xunit;

namespace Thor.Core.Session.Tests
{
    public class ProviderActivationEventSourceTests
    {
        [Fact(DisplayName = "Analyze: Inspects ProviderActivationEventSource schema")]
        public void Analyze()
        {
            // arrange
            Analyzer.EventSourceAnalyzer analyzer = new Analyzer.EventSourceAnalyzer();

            // act
            Analyzer.Report report = analyzer.Inspect(ProviderActivationEventSource.Log);

            // assert
            Assert.False(report.HasErrors);
        }

        #region Activated

        [Fact(DisplayName = "Activated: Should not write anything to the log stream")]
        public void ActivatedDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                ProviderActivationEventSource.Log.Activated("Activated-Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Activated: Should write one message to the log stream")]
        public void ActivatedEnabled()
        {
            ProviderActivationEventSource.Log.Listen((listener) =>
            {
                // arrange
                string expectedMessage = "Activated provider \"{0}\"";

                // act
                ProviderActivationEventSource.Log.Activated("Activated-Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Single(firstItem.Payload);
                Assert.Equal(1, firstItem.EventId);
                Assert.Equal(EventLevel.Verbose, firstItem.Level);
                Assert.Equal("Activated-Enabled", firstItem.Payload[0]);
            });
        }

        #endregion

        #region Activating

        [Fact(DisplayName = "Activating: Should not write anything to the log stream")]
        public void ActivatingDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                ProviderActivationEventSource.Log.Activating("Activating-Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "Activating: Should write one message to the log stream")]
        public void ActivatingEnabled()
        {
            ProviderActivationEventSource.Log.Listen((listener) =>
            {
                // arrange
                string expectedMessage = "Activating provider \"{0}\" ...";

                // act
                ProviderActivationEventSource.Log.Activating("Activating-Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Single(firstItem.Payload);
                Assert.Equal(2, firstItem.EventId);
                Assert.Equal(EventLevel.Verbose, firstItem.Level);
                Assert.Equal("Activating-Enabled", firstItem.Payload[0]);
            });
        }

        #endregion

        #region AlreadyActivated

        [Fact(DisplayName = "AlreadyActivated: Should not write anything to the log stream")]
        public void AlreadyActivatedDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                ProviderActivationEventSource.Log.AlreadyActivated("AlreadyActivated-Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "AlreadyActivated: Should write one message to the log stream")]
        public void AlreadyActivatedEnabled()
        {
            ProviderActivationEventSource.Log.Listen((listener) =>
            {
                // arrange
                string expectedMessage = "Provider \"{0}\" already activated";

                // act
                ProviderActivationEventSource.Log.AlreadyActivated("AlreadyActivated-Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Single(firstItem.Payload);
                Assert.Equal(3, firstItem.EventId);
                Assert.Equal(EventLevel.Verbose, firstItem.Level);
                Assert.Equal("AlreadyActivated-Enabled", firstItem.Payload[0]);
            });
        }

        #endregion

        #region NoInstance

        [Fact(DisplayName = "NoInstance: Should not write anything to the log stream")]
        public void NoInstanceDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                ProviderActivationEventSource.Log.NoInstance("NoInstance-Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "NoInstance: Should write one message to the log stream")]
        public void NoInstanceEnabled()
        {
            ProviderActivationEventSource.Log.Listen((listener) =>
            {
                // arrange
                string expectedMessage = "Provider \"{0}\" could not be instantiated";

                // act
                ProviderActivationEventSource.Log.NoInstance("NoInstance-Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Single(firstItem.Payload);
                Assert.Equal(4, firstItem.EventId);
                Assert.Equal(EventLevel.Verbose, firstItem.Level);
                Assert.Equal("NoInstance-Enabled", firstItem.Payload[0]);
            });
        }

        #endregion

        #region NotFound

        [Fact(DisplayName = "NotFound: Should not write anything to the log stream")]
        public void NotFoundDisabled()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // act
                ProviderActivationEventSource.Log.NotFound("NotFound-Disabled");

                // assert
                Assert.Empty(listener.OrderedEvents);
            };
        }

        [Fact(DisplayName = "NotFound: Should write one message to the log stream")]
        public void NotFoundEnabled()
        {
            ProviderActivationEventSource.Log.Listen((listener) =>
            {
                // arrange
                string expectedMessage = "Provider \"{0}\" not found";

                // act
                ProviderActivationEventSource.Log.NotFound("NotFound-Enabled");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Single(firstItem.Payload);
                Assert.Equal(5, firstItem.EventId);
                Assert.Equal(EventLevel.Verbose, firstItem.Level);
                Assert.Equal("NotFound-Enabled", firstItem.Payload[0]);
            });
        }

        #endregion
    }
}