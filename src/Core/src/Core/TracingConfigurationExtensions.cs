using System;
using System.IO;

namespace Thor.Core
{
    /// <summary>
    /// A bunch of convenient <see cref="TracingConfiguration"/> extension methods.
    /// </summary>
    public static class TracingConfigurationExtensions
    {
        /// <summary>
        /// Gets the path for storing attachments temporarily.
        /// </summary>
        /// <param name="configuration">A configuration instance.</param>
        /// <returns>A temp path for attachments.</returns>
        public static string GetAttachmentsStoragePath(
            this TracingConfiguration configuration)
        {
            return configuration.GetStoragePath("Attachments");
        }

        /// <summary>
        /// Gets the path for storing attachments temporarily.
        /// </summary>
        /// <param name="configuration">A configuration instance.</param>
        /// <returns>A temp path for attachments.</returns>
        public static string GetEventsStoragePath(
            this TracingConfiguration configuration)
        {
            return configuration.GetStoragePath("Events");
        }

        private static string GetStoragePath(
            this TracingConfiguration configuration,
            string directoryName)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return (configuration.Debug || configuration.InProcess)
                ? configuration.GetInProcessStoragePath(directoryName)
                : configuration.GetOutOfProcessStoragePath(directoryName);
        }

        private static string GetInProcessStoragePath(
            this TracingConfiguration configuration,
            string directoryName)
        {
            var path = Path.Combine(configuration.ApplicationRootPath, directoryName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        private static string GetOutOfProcessStoragePath(
            this TracingConfiguration configuration,
            string directoryName)
        {
            // note: Considering to have one global path for out-of-process
            // attachments so that just the out-of-process collector is
            // uploading attachments instead of every application is uploading
            // its own attachments.

            return configuration.GetInProcessStoragePath(directoryName);
        }
    }
}
