using System;
using Thor.Core.Testing.Utilities;
using Xunit;

namespace Thor.Core.Http.Tests
{
    public class ServerRequestScopeTests
    {
        [Fact(DisplayName = "Create: Should create a server scope")]
        public void Create()
        {
            RequestEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "PUT";
                Uri uri = new Uri("http://127.0.0.1/api/events");

                // act
                using (ServerRequestScope.Create(method, uri, null))
                {
                    // do nothing here
                }

                // assert
                Assert.Collection(listener.OrderedEvents,
                    e =>
                    {
                        Assert.Equal("Request {2} {3}", e.Message);
                        Assert.Equal(method, e.Payload[2]);
                        Assert.Equal(uri.ToString(), e.Payload[3]);
                    },
                    e =>
                    {
                        Assert.Equal("Response {3} {4}", e.Message);
                        Assert.Equal(0, e.Payload[3]);
                        Assert.Equal("UNKNOWN", e.Payload[4]);
                    });
            });
        }

        [Fact(DisplayName = "Create: Should create a server scope and throw a warning when trying to link parent scope directly")]
        public void Create_OuterScope()
        {
            RequestEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "GET";
                Uri uri = new Uri("http://127.0.0.1/api/events");

                // act
                using (Activity.Create("Outer scope"))
                {
                    using (ServerRequestScope.Create(method, uri, null))
                    {
                        // do nothing here
                    }
                }

                // assert
                Assert.Collection(listener.OrderedEvents,
                    e => Assert.Equal("Outer scopes are not allowed when creating a server-side message scope.", e.Message),
                    e =>
                    {
                        Assert.Equal("Request {2} {3}", e.Message);
                        Assert.Equal(method, e.Payload[2]);
                        Assert.Equal(uri.ToString(), e.Payload[3]);
                    },
                    e =>
                    {
                        Assert.Equal("Response {3} {4}", e.Message);
                        Assert.Equal(0, e.Payload[3]);
                        Assert.Equal("UNKNOWN", e.Payload[4]);
                    });
            });
        }
    }
}