using System;
using Thor.Core.Session.Abstractions;
using Xunit;

namespace Thor.Core.Http.Tests
{
    public class ServerRequestActivityTests
    {
        #region Create

        [Fact(DisplayName = "Create: Should throw an argument null exception for method")]
        public void Create_MethodNull()
        {
            // arrange
            const string method = null;
            Uri uri = new Uri("http://127.0.0.1/api/events");

            // act
            Action verify = () => ServerRequestActivity.Create(method, uri, null);

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
            Action verify = () => ServerRequestActivity.Create(method, uri, null);

            // assert
            Assert.Throws<ArgumentNullException>("uri", verify);
        }

        [Fact(DisplayName = "Create: Should create a server activity")]
        public void Create()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "PUT";
                Uri uri = new Uri("http://127.0.0.1/api/events");

                // act
                using (ServerRequestActivity.Create(method, uri, null))
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

        [Fact(DisplayName = "Create: Should create a server activity and throw a warning when trying to link parent activity directly")]
        public void Create_OuterActivity()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "GET";
                Uri uri = new Uri("http://127.0.0.1/api/events");

                // act
                using (Activity.Create("Outer activity"))
                {
                    using (ServerRequestActivity.Create(method, uri, null))
                    {
                        // do nothing here
                    }
                }

                // assert
                Assert.Collection(listener.OrderedEvents,
                    e => Assert.Equal("Outer activities are not allowed when creating a server-side message activity.", e.Message),
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

        [Fact(DisplayName = "Create: Should create a server activity and link it to a parent activity")]
        public void Create_ParentActivity()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "GET";
                Uri uri = new Uri("http://127.0.0.1/api/events");
                Guid parentId = Guid.NewGuid();

                // act
                using (ServerRequestActivity.Create(method, uri, parentId))
                {
                    // do nothing here
                }

                // assert
                Assert.Collection(listener.OrderedEvents,
                    e => Assert.Equal("Begin activity transfer", e.Message),
                    e =>
                    {
                        Assert.Equal("Request {2} {3}", e.Message);
                        Assert.Equal(method, e.Payload[2]);
                        Assert.Equal(uri.ToString(), e.Payload[3]);
                    },
                    e => Assert.Equal("End activity transfer", e.Message),
                    e =>
                    {
                        Assert.Equal("Response {3} {4}", e.Message);
                        Assert.Equal(0, e.Payload[3]);
                        Assert.Equal("UNKNOWN", e.Payload[4]);
                    });
            });
        }

        #endregion

        #region Create(request)

        [Fact(DisplayName = "Create(request): Should throw an argument null exception for request")]
        public void Create_HttpRequest_RequestNull()
        {
            // arrange
            HttpRequest request = null;

            // act
            Action verify = () => ServerRequestActivity.Create(request, null);

            // assert
            Assert.Throws<ArgumentNullException>("request", verify);
        }

        [Fact(DisplayName = "Create(request): Should create a server activity")]
        public void Create_HttpRequest()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                HttpRequest request = new HttpRequest
                {
                    Method = "PUT",
                    Uri = "http://127.0.0.1/api/events"
                };

                // act
                using (ServerRequestActivity.Create(request, null))
                {
                    // do nothing here
                }

                // assert
                Assert.Collection(listener.OrderedEvents,
                    e =>
                    {
                        Assert.Equal("Request {2} {3}", e.Message);
                        Assert.Equal(request.Method, e.Payload[2]);
                        Assert.Equal(request.Uri, e.Payload[3]);
                    },
                    e =>
                    {
                        Assert.Equal("Response {3} {4}", e.Message);
                        Assert.Equal(0, e.Payload[3]);
                        Assert.Equal("UNKNOWN", e.Payload[4]);
                    });
            });
        }

        [Fact(DisplayName = "Create(request): Should create a server activity and throw a warning when trying to link parent activity directly")]
        public void Create_HttpRequest_OuterActivity()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                HttpRequest request = new HttpRequest
                {
                    Method = "GET",
                    Uri = "http://127.0.0.1/api/events"
                };

                // act
                using (Activity.Create("Outer Activity"))
                {
                    using (ServerRequestActivity.Create(request, null))
                    {
                        // do nothing here
                    }
                }

                // assert
                Assert.Collection(listener.OrderedEvents,
                    e => Assert.Equal("Outer activities are not allowed when creating a server-side message activity.", e.Message),
                    e =>
                    {
                        Assert.Equal("Request {2} {3}", e.Message);
                        Assert.Equal(request.Method, e.Payload[2]);
                        Assert.Equal(request.Uri, e.Payload[3]);
                    },
                    e =>
                    {
                        Assert.Equal("Response {3} {4}", e.Message);
                        Assert.Equal(0, e.Payload[3]);
                        Assert.Equal("UNKNOWN", e.Payload[4]);
                    });
            });
        }

        [Fact(DisplayName = "Create(request): Should create a server activity and link it to a parent activity")]
        public void Create_HttpRequest_ParentActivity()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                HttpRequest request = new HttpRequest
                {
                    Method = "GET",
                    Uri = "http://127.0.0.1/api/events"
                };
                Guid parentId = Guid.NewGuid();

                // act
                using (ServerRequestActivity.Create(request, parentId))
                {
                    // do nothing here
                }

                // assert
                Assert.Collection(listener.OrderedEvents,
                    e => Assert.Equal("Begin activity transfer", e.Message),
                    e =>
                    {
                        Assert.Equal("Request {2} {3}", e.Message);
                        Assert.Equal(request.Method, e.Payload[2]);
                        Assert.Equal(request.Uri, e.Payload[3]);
                    },
                    e => Assert.Equal("End activity transfer", e.Message),
                    e =>
                    {
                        Assert.Equal("Response {3} {4}", e.Message);
                        Assert.Equal(0, e.Payload[3]);
                        Assert.Equal("UNKNOWN", e.Payload[4]);
                    });
            });
        }

        #endregion

        #region HandleException

        [Fact(DisplayName = "HandleException: Should throw an argument null exception for exception")]
        public void HandleException_ExceptionNull()
        {
            // arrange
            const string method = "GET";
            Uri uri = new Uri("http://127.0.0.1/api/events");
            Exception exception = null;
            ServerRequestActivity activity = ServerRequestActivity.Create(method, uri, null);

            // act
            Action verify = () => activity.HandleException(exception);

            // assert
            Assert.Throws<ArgumentNullException>("exception", verify);
        }

        [Fact(DisplayName = "HandleException: Should handle an exception")]
        public void HandleException()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "PUT";
                Uri uri = new Uri("http://127.0.0.1/api/events");
                Exception exception = new Exception();

                // act
                using (ServerRequestActivity activity = ServerRequestActivity.Create(method, uri, null))
                {
                    activity.HandleException(exception);
                }

                // assert
                Assert.Collection(listener.OrderedEvents,
                    e =>
                    {
                        Assert.Equal("Request {2} {3}", e.Message);
                        Assert.Equal(method, e.Payload[2]);
                        Assert.Equal(uri.ToString(), e.Payload[3]);
                    },
                    e => Assert.Equal("Internal server error occurred.", e.Message),
                    e =>
                    {
                        Assert.Equal("Response {3} {4}", e.Message);
                        Assert.Equal(0, e.Payload[3]);
                        Assert.Equal("UNKNOWN", e.Payload[4]);
                    });
            });
        }

        #endregion

        #region SetResponse

        [Fact(DisplayName = "SetResponse: Should set the response")]
        public void SetResponse()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "GET";
                Uri uri = new Uri("http://127.0.0.1/api/events");
                const int statusCode = 200;
                Guid userId = Guid.NewGuid();

                // act
                using (ServerRequestActivity activity = ServerRequestActivity.Create(method, uri, null))
                {
                    activity.SetResponse(statusCode, userId);
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
                        Assert.Equal(userId, e.Payload[2]);
                        Assert.Equal(statusCode, e.Payload[3]);
                        Assert.Equal("OK", e.Payload[4]);
                    });
            });
        }

        #endregion

        #region SetResponse(response)

        [Fact(DisplayName = "SetResponse(response): Should throw an argument null exception for response")]
        public void SetResponse_HttpResponse_ResponseNull()
        {
            // arrange
            const string method = "POST";
            Uri uri = new Uri("http://127.0.0.1/api/events");
            HttpResponse response = null;
            ServerRequestActivity activity = ServerRequestActivity.Create(method, uri, null);

            // act
            Action verify = () => activity.SetResponse(response);

            // assert
            Assert.Throws<ArgumentNullException>("response", verify);
        }

        [Fact(DisplayName = "SetResponse(response): Should set the response")]
        public void SetResponse_HttpResponse()
        {
            RequestActivityEventSource.Log.Listen(listener =>
            {
                // arrange
                const string method = "GET";
                Uri uri = new Uri("http://127.0.0.1/api/events");
                HttpResponse response = new HttpResponse
                {
                    StatusCode = 200,
                    UserId = Guid.NewGuid()
                };

                // act
                using (ServerRequestActivity activity = ServerRequestActivity.Create(method, uri, null))
                {
                    activity.SetResponse(response);
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
                        Assert.Equal(response.UserId, e.Payload[2]);
                        Assert.Equal(response.StatusCode, e.Payload[3]);
                        Assert.Equal("OK", e.Payload[4]);
                    });
            });
        }

        #endregion
    }
}