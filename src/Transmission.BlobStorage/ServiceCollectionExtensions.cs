using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Transmission.BlobStorage
{
    /// <summary>
    /// A bunch of convenient extensions methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <c>Azure BLOB Storage</c> telemetry attachment transmission services to the service collection.
        /// </summary>
        /// <param name="services">A <see cref="IServiceCollection"/> instance.</param>
        /// <param name="configuration">A <see cref="IConfiguration"/> instance.</param>
        /// <returns>The provided <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddBlobStorageTelemetryAttachmentTransmission(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return services
                .AddTracingCore(configuration)
                .Configure<BlobStorageConfiguration>(configuration.GetSection("Tracing").GetSection("BlobStorage"))
                .AddSingleton(p =>
                {
                    BlobStorageConfiguration config = p.GetRequiredService<IOptions<BlobStorageConfiguration>>()?.Value;
                    CloudStorageAccount account = CloudStorageAccount.Parse(config.ConnectionString);
                    CloudBlobClient client = account.CreateCloudBlobClient();
                    CloudBlobContainer container = client.GetContainerReference(config.AttachmentContainerName);

                    container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

                    return container;
                })
                .AddSingleton<ITransmissionSender<AttachmentDescriptor>, BlobStorageTransmissionSender>()
                .AddSingleton<ITransmissionStorage<AttachmentDescriptor>>(p =>
                {
                    TracingConfiguration config = p.GetRequiredService<IOptions<TracingConfiguration>>()?.Value;

                    return new BlobStorageTransmissionStorage(config.GetAttachmentsStoragePath());
                })
                .AddSingleton<ITelemetryAttachmentTransmitter, BlobStorageTransmitter>();
        }
    }
}