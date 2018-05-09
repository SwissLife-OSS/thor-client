using System;
using Thor.Core.Testing.Utilities;
using Xunit;

namespace Thor.Core.Http.Tests
{
    public class ClientRequestScopeTests
    {
        [Fact(DisplayName = "Create: Should create a client scope")]
        public void Create()
        {
            RequestEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "GET";
                Uri uri = new Uri("http://127.0.0.1/api/events");

                // act
                using (ClientRequestScope.Create(method, uri))
                {
                    // do nothing here
                }

                // assert
                Assert.Collection(listener.OrderedEvents,
                    e =>
                    {
                        Assert.Equal("Initiate {2} {3}", e.Message);
                        Assert.Equal(method, e.Payload[2]);
                        Assert.Equal(uri.ToString(), e.Payload[3]);
                    },
                    e =>
                    {
                        Assert.Equal("Receive {3} {4}", e.Message);
                        Assert.Equal(0, e.Payload[3]);
                        Assert.Equal("UNKNOWN", e.Payload[4]);
                    });
            });
        }

        [Fact(DisplayName = "Create: Should create and link a client scope with its parent scope")]
        public void Create_OuterScope()
        {
            RequestEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "POST";
                Uri uri = new Uri("http://127.0.0.1/api/events");

                // act
                using (Activity.Create("Outer scope"))
                {
                    using (ClientRequestScope.Create(method, uri))
                    {
                        // do nothing here
                    }
                }

                // assert
                Assert.Collection(listener.OrderedEvents,
                    e => Assert.Equal("Begin activity transfer", e.Message),
                    e =>
                    {
                        Assert.Equal("Initiate {2} {3}", e.Message);
                        Assert.Equal(method, e.Payload[2]);
                        Assert.Equal(uri.ToString(), e.Payload[3]);
                    },
                    e => Assert.Equal("End activity transfer", e.Message),
                    e =>
                    {
                        Assert.Equal("Receive {3} {4}", e.Message);
                        Assert.Equal(0, e.Payload[3]);
                        Assert.Equal("UNKNOWN", e.Payload[4]);
                    });
            });
        }
    }
}