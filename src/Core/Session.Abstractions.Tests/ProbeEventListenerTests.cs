using System;
using System.Diagnostics.Tracing;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Thor.Core.Session.Abstractions.Tests
{
    public class ProbeEventListenerTests
    {
        private readonly ITestOutputHelper _output;

        public ProbeEventListenerTests(ITestOutputHelper output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }

        [Fact(DisplayName = "Enqueue: Should enqueue events in the right order")]
        public void Enqueue()
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                // arrange
                listener.EnableEvents(ProbeEventListenerEventSource.Log, EventLevel.Verbose);
                listener.EventSourceCreated += (sender, eventArgs) =>
                    _output.WriteLine($"Created event source: {eventArgs.EventSource.Name}");

                // act
                ProbeEventListenerEventSource.Log.Foo("1");
                ProbeEventListenerEventSource.Log.Bar("2");
                ProbeEventListenerEventSource.Log.Foo("3");

                // assert
                Assert.Collection(listener.OrderedEvents,
                    e =>
                    {
                        Assert.Equal("Foo", e.EventName);
                        Assert.Equal("1", e.Payload.Single());
                    },
                    e =>
                    {
                        Assert.Equal("Bar", e.EventName);
                        Assert.Equal("2", e.Payload.Single());
                    },
                    e =>
                    {
                        Assert.Equal("Foo", e.EventName);
                        Assert.Equal("3", e.Payload.Single());
                    });
            }
        }
    }
}