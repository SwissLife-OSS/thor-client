using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Moq;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Http.Tests
{
    public class AttachmentDispatcherExtensionsTests
    {
        #region Dispatch (HttpRequest)

        [Fact(DisplayName = "Dispatch: Should not throw an argument null exception for dispatcher")]
        public void Dispatch_HttpRequest_DispatcherNull()
        {
            // arrange
            AttachmentId id = AttachmentId.NewId();
            AttachmentDispatcher dispatcher = null;
            string payloadName = "Name-7171";
            HttpRequest payloadValue = new HttpRequest();

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Dispatch: Should not throw an argument exception for id")]
        public void Dispatch_HttpRequest_IdEmpty()
        {
            // arrange
            AttachmentId id = AttachmentId.Empty;
            AttachmentDispatcher dispatcher = AttachmentDispatcher.Instance;
            string payloadName = "Name-6161";
            HttpRequest payloadValue = new HttpRequest();

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Dispatch: Should not throw an argument null exception for payloadName")]
        public void Dispatch_HttpRequest_PayloadNameNull()
        {
            // arrange
            AttachmentId id = AttachmentId.NewId();
            AttachmentDispatcher dispatcher = AttachmentDispatcher.Instance;
            string payloadName = null;
            HttpRequest payloadValue = new HttpRequest();

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Dispatch: Should not throw an argument null exception for payloadValue")]
        public void Dispatch_HttpRequest_PayloadValueNull()
        {
            // arrange
            AttachmentId id = AttachmentId.NewId();
            AttachmentDispatcher dispatcher = AttachmentDispatcher.Instance;
            string payloadName = "Name-5151";
            HttpRequest payloadValue = null;

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));

        }

        [Fact(DisplayName = "Dispatch: Should call the internal dispatch method once when the body is empty")]
        public void Dispatch_HttpRequest_BodyEmpty()
        {
            // arrange
            int callCount = 0;
            AttachmentId id = AttachmentId.NewId();
            Mock<ITelemetryAttachmentTransmitter> transmitter = new Mock<ITelemetryAttachmentTransmitter>();

            transmitter
                .Setup(t => t.Enqueue(It.IsAny<AttachmentDescriptor>()))
                .Callback((AttachmentDescriptor d) =>
                {
                    if (d.Id == id)
                    {
                        Interlocked.Increment(ref callCount);
                    }
                });

            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>
            {
                transmitter.Object
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);
            string payloadName = "Name-4141";
            HttpRequest payloadValue = new HttpRequest();

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.Equal(1, callCount);
        }

        [Fact(DisplayName = "Dispatch: Should call the internal dispatch method twice")]
        public void Dispatch_HttpRequest()
        {
            // arrange
            int callCount = 0;
            AttachmentId id = AttachmentId.NewId();
            Mock<ITelemetryAttachmentTransmitter> transmitter = new Mock<ITelemetryAttachmentTransmitter>();

            transmitter
                .Setup(t => t.Enqueue(It.IsAny<AttachmentDescriptor>()))
                .Callback((AttachmentDescriptor d) =>
                {
                    if (d.Id == id)
                    {
                        Interlocked.Increment(ref callCount);
                    }
                });

            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>
            {
                transmitter.Object
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);
            string payloadName = "Name-3131";
            HttpRequest payloadValue = new HttpRequest
            {
                Body = new MemoryStream(new byte[] { 4, 5, 6 })
            };

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.Equal(2, callCount);
        }

        #endregion

        #region Dispatch (HttpResponse)

        [Fact(DisplayName = "Dispatch: Should not throw an argument null exception for dispatcher")]
        public void Dispatch_HttpResponse_DispatcherNull()
        {
            // arrange
            AttachmentId id = AttachmentId.NewId();
            AttachmentDispatcher dispatcher = null;
            string payloadName = "Name-7272";
            HttpResponse payloadValue = new HttpResponse();

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Dispatch: Should not throw an argument exception for id")]
        public void Dispatch_HttpResponse_IdEmpty()
        {
            // arrange
            AttachmentId id = AttachmentId.Empty;
            AttachmentDispatcher dispatcher = AttachmentDispatcher.Instance;
            string payloadName = "Name-6262";
            HttpResponse payloadValue = new HttpResponse();

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Dispatch: Should not throw an argument null exception for payloadName")]
        public void Dispatch_HttpResponse_PayloadNameNull()
        {
            // arrange
            AttachmentId id = AttachmentId.NewId();
            AttachmentDispatcher dispatcher = AttachmentDispatcher.Instance;
            string payloadName = null;
            HttpResponse payloadValue = new HttpResponse();

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
        }

        [Fact(DisplayName = "Dispatch: Should not throw an argument null exception for payloadValue")]
        public void Dispatch_HttpResponse_PayloadValueNull()
        {
            // arrange
            AttachmentId id = AttachmentId.NewId();
            AttachmentDispatcher dispatcher = AttachmentDispatcher.Instance;
            string payloadName = "Name-5252";
            HttpResponse payloadValue = null;

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));

        }

        [Fact(DisplayName = "Dispatch: Should call the internal dispatch method once when the body is empty")]
        public void Dispatch_HttpResponse_BodyEmpty()
        {
            // arrange
            int callCount = 0;
            AttachmentId id = AttachmentId.NewId();
            Mock<ITelemetryAttachmentTransmitter> transmitter = new Mock<ITelemetryAttachmentTransmitter>();

            transmitter
                .Setup(t => t.Enqueue(It.IsAny<AttachmentDescriptor>()))
                .Callback((AttachmentDescriptor d) =>
                {
                    if (d.Id == id)
                    {
                        Interlocked.Increment(ref callCount);
                    }
                });

            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>
            {
                transmitter.Object
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);
            string payloadName = "Name-4242";
            HttpResponse payloadValue = new HttpResponse();

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.Equal(1, callCount);
        }

        [Fact(DisplayName = "Dispatch: Should call the internal dispatch method once because the body dispatch is disabled")]
        public void Dispatch_HttpResponse()
        {
            // arrange
            int callCount = 0;
            AttachmentId id = AttachmentId.NewId();
            Mock<ITelemetryAttachmentTransmitter> transmitter = new Mock<ITelemetryAttachmentTransmitter>();

            transmitter
                .Setup(t => t.Enqueue(It.IsAny<AttachmentDescriptor>()))
                .Callback((AttachmentDescriptor d) =>
                {
                    if (d.Id == id)
                    {
                        Interlocked.Increment(ref callCount);
                    }
                });

            HashSet<ITelemetryAttachmentTransmitter> transmitters = new HashSet<ITelemetryAttachmentTransmitter>
            {
                transmitter.Object
            };
            AttachmentDispatcher dispatcher = new AttachmentDispatcher(transmitters);
            string payloadName = "Name-3232";
            HttpResponse payloadValue = new HttpResponse
            {
                Body = new MemoryStream(new byte[] { 1, 2, 3 })
            };

            // act
            Action verify = () => dispatcher.Dispatch(id, payloadName, payloadValue);

            // assert
            Assert.Null(Record.Exception(verify));
            Assert.Equal(1, callCount);
        }

        #endregion
    }
}