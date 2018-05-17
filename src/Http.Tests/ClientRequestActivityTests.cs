using System;
using Thor.Core.Session.Abstractions;
using Xunit;

namespace Thor.Core.Http.Tests
{
    public class ClientRequestActivityTests
    {
        #region Create

        [Fact(DisplayName = "Create: Should throw an argument null exception for method")]
        public void Create_MethodNull()
        {
            // arrange
            const string method = null;
            Uri uri = new Uri("http://127.0.0.1/api/events");

            // act
            Action verify = () => ClientRequestActivity.Create(method, uri);

            // assert
            Assert.Throws<ArgumentNullException>("method", verify);
        }

        [Fact(DisplayName = "Create: Should throw an argument null exception for uri")]
        public void Create_UriNull()
        {
            // arrange
            const string method = "GET";
            Uri uri = null;

            // act
            Action verify = () => ClientRequestActivity.Create(method, uri);

            // assert
            Assert.Throws<ArgumentNullException>("uri", verify);
        }

        [Fact(DisplayName = "Create: Should create a client activity")]
        public void Create()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "GET";
                Uri uri = new Uri("http://127.0.0.1/api/events");

                // act
                using (ClientRequestActivity.Create(method, uri))
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

        [Fact(DisplayName = "Create: Should create and link a client activity with its parent activity")]
        public void Create_Outeractivity()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "POST";
                Uri uri = new Uri("http://127.0.0.1/api/events");

                // act
                using (Activity.Create("Outer activity"))
                {
                    using (ClientRequestActivity.Create(method, uri))
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

        #endregion

        #region SetResponse

        [Fact(DisplayName = "SetResponse: Should set the response proper")]
        public void SetResponse()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "GET";
                Uri uri = new Uri("http://127.0.0.1/api/events");
                Guid userId = Guid.NewGuid();

                // act
                using (ClientRequestActivity activity = ClientRequestActivity.Create(method, uri))
                {
                    activity.SetResponse(200, userId);
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
                        Assert.Equal(userId, e.Payload[2]);
                        Assert.Equal(200, e.Payload[3]);
                        Assert.Equal("OK", e.Payload[4]);
                    });
            });
        }

        #endregion
    }
}