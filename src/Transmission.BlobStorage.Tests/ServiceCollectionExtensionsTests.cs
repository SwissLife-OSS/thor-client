using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Thor.Core.Transmission.Abstractions;
using Xunit;

namespace Thor.Core.Transmission.BlobStorage.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        #region AddBlobStorageTelemetryAttachmentTransmission

        [Fact(DisplayName = "AddBlobStorageTelemetryAttachmentTransmission: Should throw an argument null exception for services")]
        public void AddBlobStorageTelemetryAttachmentTransmission_ServicesNull()
        {
            // arrange
            IServiceCollection services = null;
            IConfiguration configuration = new Mock<IConfiguration>().Object;

            // act
            Action verify = () => services.AddBlobStorageTelemetryAttachmentTransmission(configuration);

            // arrange
            Assert.Throws<ArgumentNullException>("services", verify);
        }

        [Fact(DisplayName = "AddBlobStorageTelemetryAttachmentTransmission: Should throw an argument null exception for configuration")]
        public void AddBlobStorageTelemetryAttachmentTransmission_ConfigurationNull()
        {
            // arrange
            IServiceCollection services = new Mock<IServiceCollection>().Object;
            IConfiguration configuration = null;

            // act
            Action verify = () => services.AddBlobStorageTelemetryAttachmentTransmission(configuration);

            // arrange
            Assert.Throws<ArgumentNullException>("configuration", verify);
        }

        [Fact(DisplayName = "AddBlobStorageTelemetryAttachmentTransmission: Resolve telemetry transmitter",
            Skip = "Because a valid storage connection string is required")]
        public void AddBlobStorageTelemetryAttachmentTransmission()
        {
            // arrange
            IServiceCollection services = new ServiceCollection();
            IConfigurationBuilder builder = new ConfigurationBuilder();
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                {"Tracing:BlobStorage:ConnectionString", Constants.FakeConnectionString}
            };

            builder.AddInMemoryCollection(data);

            IConfiguration configuration = builder.Build();

            // act
            services.AddBlobStorageTelemetryAttachmentTransmission(configuration);

            // assert
            ServiceProvider provider = services.BuildServiceProvider();

            Assert.IsType<BlobStorageTransmitter>(provider.GetService<ITelemetryAttachmentTransmitter>());
        }

        #endregion
    }
}