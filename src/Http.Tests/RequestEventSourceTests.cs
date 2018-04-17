using System;
using System.Diagnostics.Tracing;
using System.Linq;
using Thor.Analyzer;
using Xunit;

namespace Thor.Core.Http.Tests
{
    public class RequestEventSourceTests
    {
        [Fact(DisplayName = "RequestEventSource: Inspect schema")]
        public void Inspect()
        {
            // arrange
            EventSourceAnalyzer analyzer = new EventSourceAnalyzer();

            // act
            Report report = analyzer.Inspect(RequestEventSource.Log);

            // assert
            Assert.False(report.HasErrors);
        }

        #region Send/Receive Events

        [Fact(DisplayName = "Send: Should log a client request start")]
        public void Send()
        {
            ProbeEvents(RequestEventSource.Log, listener =>
            {
                // arrange
                const string expectedMessage = "Initiate GET http://127.0.0.1/api/events";
                Guid activityId = Guid.NewGuid();

                // act
                RequestEventSource.Log.Send(activityId, "GET", "http://127.0.0.1/api/events");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.GetFormattedMessage() == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Equal(EventLevel.LogAlways, firstItem.Level);
                AssertItem(firstItem, 4, activityId, expectedMessage);
            });
        }

        [Fact(DisplayName = "Send: Should log a client request stop")]
        public void Receive()
        {
            ProbeEvents(RequestEventSource.Log, listener =>
            {
                // arrange
                const string expectedMessage = "Receive 404 NOTFOUND";
                Guid userId = Guid.NewGuid();
                Guid activityId = Guid.NewGuid();

                // act
                RequestEventSource.Log.Receive(activityId, userId, 404, "NOTFOUND");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.GetFormattedMessage() == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Equal(EventLevel.LogAlways, firstItem.Level);
                AssertItem(firstItem, 5, activityId, userId, expectedMessage);
            });
        }

        #endregion

        #region Start/Stop Events

        [Fact(DisplayName = "Start: Should log a server reqeust start")]
        public void Start()
        {
            ProbeEvents(RequestEventSource.Log, listener =>
            {
                // arrange
                const string expectedMessage = "Request GET http://127.0.0.1/api/events";
                Guid activityId = Guid.NewGuid();

                // act
                RequestEventSource.Log.Start(activityId, "GET", "http://127.0.0.1/api/events");

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.GetFormattedMessage() == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Equal(EventLevel.LogAlways, firstItem.Level);
                AssertItem(firstItem, 5, activityId, expectedMessage);
            });
        }

        [Fact(DisplayName = "Start: Should log a server reqeust start (HttpRequest)")]
        public void Start_HttpRequest()
        {
            ProbeEvents(RequestEventSource.Log, listener =>
            {
                // arrange
                const string expectedMessage = "Request GET http://127.0.0.1/api/events";
                Guid activityId = Guid.NewGuid();
                HttpRequest request = new HttpRequest
                {
                    Method = "GET",
                    Uri = "http://127.0.0.1/api/events"
                };

                // act
                RequestEventSource.Log.Start(activityId, request);

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.GetFormattedMessage() == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Equal(EventLevel.LogAlways, firstItem.Level);
                AssertItem(firstItem, 5, activityId, expectedMessage);
            });
        }

        [Fact(DisplayName = "Stop: Should log a server reqeust stop")]
        public void Stop()
        {
            ProbeEvents(RequestEventSource.Log, listener =>
            {
                // arrange
                const string expectedMessage = "Response 200 OK";
                Guid userId = Guid.NewGuid();
                Guid activityId = Guid.NewGuid();

                // act
                RequestEventSource.Log.Stop(activityId, userId, 200);

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.GetFormattedMessage() == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Equal(EventLevel.LogAlways, firstItem.Level);
                AssertItem(firstItem, 6, activityId, userId, expectedMessage);
            });
        }

        [Fact(DisplayName = "Stop: Should log a server reqeust stop (HttpResponse)")]
        public void Stop_HttpResponse()
        {
            ProbeEvents(RequestEventSource.Log, listener =>
            {
                // arrange
                const string expectedMessage = "Response 200 OK";
                Guid userId = Guid.NewGuid();
                Guid activityId = Guid.NewGuid();
                HttpResponse response = new HttpResponse
                {
                    StatusCode = 200,
                    UserId = userId
                };

                // act
                RequestEventSource.Log.Stop(activityId, response);

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.GetFormattedMessage() == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Equal(EventLevel.LogAlways, firstItem.Level);
                AssertItem(firstItem, 6, activityId, userId, expectedMessage);
            });
        }

        [Fact(DisplayName = "Stop: Should log a server reqeust stop without a status code and user id (HttpResponse)")]
        public void Stop_HttpResponse_NoStatusAndUserId()
        {
            ProbeEvents(RequestEventSource.Log, listener =>
            {
                // arrange
                const string expectedMessage = "Response 0 UNKNOWN";
                Guid activityId = Guid.NewGuid();
                HttpResponse response = new HttpResponse
                {
                    StatusCode = 0,
                    UserId = Guid.Empty
                };

                // act
                RequestEventSource.Log.Stop(activityId, response);

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.GetFormattedMessage() == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Equal(EventLevel.LogAlways, firstItem.Level);
                AssertItem(firstItem, 6, activityId, null, expectedMessage);
            });
        }

        [Fact(DisplayName = "Stop: Should log a server reqeust stop with response null (HttpResponse)")]
        public void Stop_HttpResponse_Null()
        {
            ProbeEvents(RequestEventSource.Log, listener =>
            {
                // arrange
                const string expectedMessage = "Response 0 UNKNOWN";
                Guid activityId = Guid.NewGuid();
                HttpResponse response = null;

                // act
                RequestEventSource.Log.Stop(activityId, response);

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.GetFormattedMessage() == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Equal(EventLevel.LogAlways, firstItem.Level);
                AssertItem(firstItem, 6, activityId, null, expectedMessage);
            });
        }

        #endregion

        #region Begin/End Transfer Events

        [Fact(DisplayName = "BeginTransfer: Should log a begin transfer")]
        public void BeginTransfer()
        {
            ProbeEvents(RequestEventSource.Log, listener =>
            {
                // arrange
                const string expectedMessage = "Begin activity transfer";
                Guid activityId = Guid.NewGuid();

                // act
                RequestEventSource.Log.BeginTransfer(activityId);

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.GetFormattedMessage() == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Equal(EventLevel.LogAlways, firstItem.Level);
                AssertItem(firstItem, 2, activityId, expectedMessage);
            });
        }

        [Fact(DisplayName = "EndTransfer: Should log an end transfer")]
        public void EndTransfer()
        {
            ProbeEvents(RequestEventSource.Log, listener =>
            {
                // arrange
                const string expectedMessage = "End activity transfer";
                Guid activityId = Guid.NewGuid();
                Guid relatedActivityId = Guid.NewGuid();

                // act
                RequestEventSource.Log.EndTransfer(activityId, relatedActivityId);

                // assert
                EventWrittenEventArgs firstItem = listener
                    .OrderedEvents
                    .FirstOrDefault(e => e.Message == expectedMessage);

                Assert.NotNull(firstItem);
                Assert.Equal(EventLevel.LogAlways, firstItem.Level);
                Assert.Equal(relatedActivityId, firstItem.RelatedActivityId);
                AssertItem(firstItem, 2, activityId, expectedMessage);
            });
        }

        #endregion

        private static void AssertItem(EventWrittenEventArgs item, int expectedCount,
            Guid expectedActivityId, Guid? expectedUserId, string expectedMessage)
        {
            AssertItem(item, expectedCount, expectedActivityId, expectedMessage);

            Assert.Equal(expectedUserId, item.GetUserId());
        }

        private static void AssertItem(EventWrittenEventArgs item, int expectedCount,
            Guid expectedActivityId, string expectedMessage)
        {
            Assert.Equal(0, item.GetApplicationId());
            Assert.Equal(expectedActivityId, item.GetActivityId());
            Assert.Equal(EventSourceNames.Request, item.EventSource.Name);

            if (expectedCount == 0)
            {
                Assert.Null(item.Payload);
            }
            else
            {
                Assert.Equal(expectedCount, item.Payload.Count);
            }
        }

        private static void ProbeEvents(EventSource eventSource,
            Action<ProbeEventListener> execute)
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                try
                {
                    listener.EnableEvents(eventSource, EventLevel.Verbose);
                    execute(listener);
                }
                finally
                {
                    listener.DisableEvents(eventSource);
                }
            }
        }
    }
}