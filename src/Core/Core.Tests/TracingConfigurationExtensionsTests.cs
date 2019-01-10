using System;
using Xunit;

namespace Thor.Core.Tests
{
    public class TracingConfigurationExtensionsTests
    {
        #region GetAttachmentsStoragePath

        [Fact(DisplayName = "GetAttachmentsStoragePath: Should throw an argument null exception for configuration")]
        public void GetAttachmentsStoragePath_ConfigurationNull()
        {
            // arrange
            TracingConfiguration configuration = null;

            // act
            Action verify = () => configuration.GetAttachmentsStoragePath();

            // assert
            Assert.Throws<ArgumentNullException>("configuration", verify);
        }

        [Fact(DisplayName = "GetAttachmentsStoragePath: Should not throw any exception")]
        public void GetAttachmentsStoragePath_NoException()
        {
            // arrange
            TracingConfiguration configuration = new TracingConfiguration();

            // act
            Action verify = () => configuration.GetAttachmentsStoragePath();

            // assert
            Assert.Null(Record.Exception(verify));
        }

        #endregion
    }
}