using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Moq;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.EventHub.Tests
{
    public class EventHubTransmitterTests
    {
        #region Constructor

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for client")]
        public void Constructor_SourceNull()
        {
            // assert
            EventHubClient client = null;

            // act
            Action verify = () => new EventHubTransmitter(client);

            // arrange
            Assert.Throws<ArgumentNullException>("client", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_NoException()
        {
            // assert
            EventHubClient client = EventHubClient.CreateFromConnectionString(Constants.FakeConnectionString);

            // act
            Action verify = () => new EventHubTransmitter(client);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        #endregion

        #region Enqueue

        [Fact(DisplayName = "Enqueue: Should throw an argument null exception for data")]
        public void Enqueue_DataNull()
        {
            // arrange
            ITransmissionBuffer<EventData> buffer = new Mock<ITransmissionBuffer<EventData>>().Object;
            ITransmissionSender<EventData> sender = new Mock<ITransmissionSender<EventData>>().Object;
            ITelemetryEventTransmitter transmitter = new EventHubTransmitter(buffer, sender);
            TelemetryEvent data = null;

            // act
            Action verify = () => transmitter.Enqueue(data);

            // assert
            Assert.Throws<ArgumentNullException>("data", verify);
        }

        [Fact(DisplayName = "Enqueue: Should not throw any exception")]
        public void Enqueue_NoException()
        {
            // assert
            ITransmissionBuffer<EventData> buffer = new Mock<ITransmissionBuffer<EventData>>().Object;
            ITransmissionSender<EventData> sender = new Mock<ITransmissionSender<EventData>>().Object;
            TelemetryEvent data = new TelemetryEvent();

            // act
            Action verify = () => new EventHubTransmitter(buffer, sender);

            // arrange
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Enqueue: Should enqueue data and transfer it")]
        public void Enqueue_TransmissionFlow()
        {
            // assert
            ManualResetEventSlim resetEvent = new ManualResetEventSlim();
            Mock<ITransmissionBuffer<EventData>> buffer = new Mock<ITransmissionBuffer<EventData>>();
            Mock<ITransmissionSender<EventData>> sender = new Mock<ITransmissionSender<EventData>>();
            ConcurrentQueue<EventData> bufferQueue = new ConcurrentQueue<EventData>();

            buffer
                .Setup(t => t.EnqueueAsync(It.IsAny<EventData>(), It.IsAny<CancellationToken>()))
                .Callback((EventData d, CancellationToken t) => bufferQueue.Enqueue(d));
            buffer
                .Setup(t => t.DequeueAsync(It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    int count = 0;
                    List<EventData> results = new List<EventData>();

                    while (bufferQueue.TryDequeue(out EventData d) && count < 10)
                    {
                        results.Add(d);
                        count++;
                    }

                    return Task.FromResult(results.AsEnumerable());
                });
            sender
                .Setup(t => t.SendAsync(It.IsAny<IEnumerable<EventData>>(), It.IsAny<CancellationToken>()))
                .Callback(() => resetEvent.Set());

            ITelemetryEventTransmitter transmitter = new EventHubTransmitter(buffer.Object, sender.Object);
            TelemetryEvent data = new TelemetryEvent();

            // act
            transmitter.Enqueue(data);

            // arrange
            resetEvent.Wait(TimeSpan.FromSeconds(5));

            sender.Verify(s => s.SendAsync(
                It.Is((IEnumerable<EventData> d) => d.Count() == 1),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Dispose

        [Fact(DisplayName = "Dispose: Should work like a charm")]
        public void Dispose()
        {
            // arrange
            ITransmissionBuffer<EventData> buffer = new Mock<ITransmissionBuffer<EventData>>().Object;
            ITransmissionSender<EventData> sender = new Mock<ITransmissionSender<EventData>>().Object;
            EventHubTransmitter transmitter = new EventHubTransmitter(buffer, sender);

            // act
            Action verify = () => transmitter.Dispose();

            // assert
            Assert.Null(Record.Exception(verify));
        }

        #endregion
    }
}