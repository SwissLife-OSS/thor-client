using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Thor.Core.Http
{
    /// <summary>
    /// Represents a <c>HTTP</c> request.
    /// </summary>
    public class HttpRequest
    {
        /// <summary>
        /// Gets or set the Accept header.
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// Gets or set the body Stream.
        /// </summary>
        [JsonIgnore]
        public Stream Body { get; set; }

        /// <summary>
        /// Gets or sets the Cache-Control header.
        /// </summary>
        public string CacheControl { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token for the request.
        /// </summary>
        public bool Cancelled { get; set; }

        /// <summary>
        /// Gets or sets the Content-Type header.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets the collection of Cookies for this request.
        /// </summary>
        public IDictionary<string, string> Cookies { get; set; }

        /// <summary>
        /// Gets the request headers.
        /// </summary>
        public Dictionary<string, string[]> Headers { get; set; }

        /// <summary>
        /// Gets or set the Host header. May include the port.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the scheme is https.
        /// </summary>
        public bool IsSecure { get; set; }

        /// <summary>
        /// Gets or set the server local IP address.
        /// </summary>
        public string LocalIpAddress { get; set; }

        /// <summary>
        /// Gets or set the server local port.
        /// </summary>
        public int? LocalPort { get; set; }

        /// <summary>
        /// Gets or sets the Media-Type header.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// Gets or set the HTTP method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or set the request path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or set the request path base.
        /// </summary>
        public string PathBase { get; set; }

        /// <summary>
        /// Gets or set the request protocol.
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Gets the query value collection parsed from the query string.
        /// </summary>
        public Dictionary<string, string[]> Query { get; set; }

        /// <summary>
        /// Gets or set the query string.
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or set the server remote IP address.
        /// </summary>
        public string RemoteIpAddress { get; set; }

        /// <summary>
        /// Gets or set the server remote port.
        /// </summary>
        public int? RemotePort { get; set; }

        /// <summary>
        /// Gets or set the HTTP request scheme.
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// Gets the uniform resource identifier (URI) associated with the request.
        /// </summary>
        [JsonIgnore]
        public string Uri { get; set; }
    }
}