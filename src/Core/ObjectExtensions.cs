using System;
using System.Text;
using Newtonsoft.Json;

namespace Thor.Core
{
    /// <summary>
    /// A bunch of convenient extensions for <see cref="object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        /// <summary>
        /// Serializes an object for <see cref="IAttachment.Value"/>.
        /// </summary>
        /// <param name="content">An object.</param>
        /// <returns>A UTF8 byte array.</returns>
        public static byte[] SerializeAttachmentContent(this object content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            string json = JsonConvert.SerializeObject(content, _settings);

            return Encoding.UTF8.GetBytes(json);
        }
    }
}