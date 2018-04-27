using System;
using Microsoft.Azure.EventHubs;
using Thor.Core.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.EventHub.Tests
{
    public class TelemetryEventExtensionsTests
    {
        #region Map

        [Fact(DisplayName = "Map: Should throw an argument null exception for source")]
        public void Map_SourceNull()
        {
            // assert
            TelemetryEvent source = null;

            // act
            Action verify = () => source.Map();

            // arrange
            Assert.Throws<ArgumentNullException>("source", verify);
        }

        [Fact(DisplayName = "Map: Should map successfully")]
        public void Map_Success()
        {
            // assert
            TelemetryEvent source = new TelemetryEvent();

            // act
            EventData destination = source.Map();

            // arrange
            Assert.NotNull(destination);
        }

        #endregion
    }
}