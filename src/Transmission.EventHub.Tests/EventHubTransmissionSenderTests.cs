using System;
using Microsoft.Azure.EventHubs;
using Xunit;

namespace Thor.Core.Transmission.EventHub.Tests
{
    public class EventHubTransmissionSenderTests
    {
        #region Constructor

        [Fact(DisplayName = "Constructor: Should throw an argument null exception for client")]
        public void Constructor_SourceNull()
        {
            // assert
            EventHubClient client = null;

            // act
            Action verify = () => new EventHubTransmissionSender(client);

            // arrange
            Assert.Throws<ArgumentNullException>("client", verify);
        }

        [Fact(DisplayName = "Constructor: Should not throw any exception",
            Skip = "We need a valid EventHub connection string")]
        public void Constructor_Success()
        {
            // assert
            EventHubClient client = EventHubClient.CreateFromConnectionString("connstring");

            // act
            Action verify = () => new EventHubTransmissionSender(client);

            // arrange
            Exception exception = Record.Exception(verify);
            Assert.NotNull(exception);
        }

        #endregion
    }
}