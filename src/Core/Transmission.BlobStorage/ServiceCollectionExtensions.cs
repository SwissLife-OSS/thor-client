using System;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection AddBlobStorageTelemetryAttachmentTransmission(
            this IServiceCollection services,
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

            BlobStorageConfiguration blobStorageConfiguration = configuration
                .GetSection("Tracing")
                .GetSection("BlobStorage")
                .Get<BlobStorageConfiguration>();

            AttachmentsOptions attachmentsOptions = configuration
                .GetSection("Tracing")
                .GetSection("Attachments")
                .Get<AttachmentsOptions>() ?? new AttachmentsOptions();

            return services
                .AddTracingCore(configuration)
                .AddSingleton(blobStorageConfiguration)
                .AddSingleton(attachmentsOptions)
                .AddSingleton(p =>
                {
                    CloudStorageAccount account = CloudStorageAccount.Parse(blobStorageConfiguration.ConnectionString);
                    CloudBlobClient client = account.CreateCloudBlobClient();
                    CloudBlobContainer container = client.GetContainerReference(blobStorageConfiguration.AttachmentContainerName);

                    container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

                    return container;
                })
                .AddSingleton<IBlobContainer, BlobContainer>()
                .AddSingleton<IMemoryBuffer<AttachmentDescriptor>>(sp => new MemoryBuffer<AttachmentDescriptor>(attachmentsOptions.Buffer))
                .AddSingleton<ITransmissionSender<AttachmentDescriptor>, BlobStorageTransmissionSender>()
                .AddSingleton<ITransmissionStorage<AttachmentDescriptor>>(p =>
                {
                    TracingConfiguration tracingConfiguration = p.GetRequiredService<TracingConfiguration>();
                    return new BlobStorageTransmissionStorage(tracingConfiguration.GetAttachmentsStoragePath());
                })
                .AddSingleton<ITelemetryAttachmentTransmitter, BlobStorageTransmitter>()
                .AddSingleton<IAttachmentTransmissionInitializer, AttachmentTransmissionInitializer>();
        }
    }
}
