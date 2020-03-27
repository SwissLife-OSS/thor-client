using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Thor.Core.Abstractions
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
        /// Deserializes an UTF8 byte array to its original value.
        /// </summary>
        /// <typeparam name="TDestination">The destination type.</typeparam>
        /// <param name="source">A UTF8 byte array.</param>
        /// <returns>A object.</returns>
        public static TDestination Deserialize<TDestination>(this byte[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            string json = Encoding.UTF8.GetString(source);

            return JsonConvert.DeserializeObject<TDestination>(json);
        }

        /// <summary>
        /// Serializes an object to a UTF8 byte array.
        /// </summary>
        /// <param name="source">An object instance.</param>
        /// <returns>A UTF8 byte array.</returns>
        public static byte[] Serialize(this object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            string json = null;

            // TODO : this is a workaround to ensure that even if we cannot serialize a payload there is no error.
            try
            {
                json = JsonConvert.SerializeObject(source, _settings);
            }
            catch
            {
                if(source is Exception ex)
                {
                    var error = new Dictionary<string, string>();
                    error["message"] = ex.Message;
                    error["stackTrace"] = ex.StackTrace;
                    json = JsonConvert.SerializeObject(error, _settings);
                }
                else
                {
                    json  = "{ \"message\": \"Thor is unable to serialize the object " +
                        $"`{source.GetType().FullName}`.\" }}";
                }
            }

            return Encoding.UTF8.GetBytes(json);
        }
    }
}
