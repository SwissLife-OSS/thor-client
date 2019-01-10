using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Thor.Core.Http
{
    /// <summary>
    /// Represents a <c>HTTP</c> response.
    /// </summary>
    public class HttpResponse
    {
        /// <summary>
        /// Gets or set the body Stream.
        /// </summary>
        [JsonIgnore]
        public Stream Body { get; set; }

        /// <summary>
        /// Gets or sets the Content-Length header.
        /// </summary>
        public long? ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the Content-Type header.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the E-Tag header.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the Expires header.
        /// </summary>
        public DateTimeOffset? Expires { get; set; }

        /// <summary>
        /// Gets the response header collection.
        /// </summary>
        public Dictionary<string, string[]> Headers { get; set; }

        /// <summary>
        /// Gets or sets the response protocol.
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the the optional HTTP reason phrase.
        /// </summary>
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// Gets or sets the optional HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}