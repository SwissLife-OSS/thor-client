using System;
using Microsoft.Azure.EventHubs;
using Xunit;

namespace Thor.Core.Transmission.EventHub.Tests
{
    public class EventHubTransmissionBufferTests
    {
        #region Constructor

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for client")]
        public void Constructor_SourceNull()
        {
            // assert
            EventHubClient client = null;

            // act
            Action verify = () => new EventHubTransmissionBuffer(client);

            // arrange
            Assert.Throws<ArgumentNullException>("client", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception")]
        public void Constructor_Success()
        {
            // assert
            EventHubClient client = EventHubClient.CreateFromConnectionString(Constants.FakeConnectionString);

            // act
            Action verify = () => new EventHubTransmissionBuffer(client);

            // arrange
            Exception exception = Record.Exception(verify);
            Assert.Null(exception);
        }

        #endregion
    }
}