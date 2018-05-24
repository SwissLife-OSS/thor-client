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
        public static string GetAttachmentsStoragePath(this TracingConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return (configuration.Debug || configuration.InProcess)
                ? configuration.GetInProcessStoragePath()
                : configuration.GetOutOfProcessStoragePath();
        }

        private static string GetInProcessStoragePath(this TracingConfiguration configuration)
        {
            string path = Path.Combine(configuration.ApplicationRootPath, "Attachments");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        private static string GetOutOfProcessStoragePath(this TracingConfiguration configuration)
        {
            // todo: implement
            throw new NotImplementedException();
        }
    }
}