using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Thor.Core.Aspnet
{
    /// <summary>
    /// A middleware for <c>aspnet</c> that contains the logic to trace <c>HTTP</c> requests.
    /// </summary>
    public class TracingMiddleware
    {
        private readonly TracingMiddlewareConfiguration _configuration;
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="TracingMiddleware"/> class.
        /// </summary>
        /// <param name="next">A delegate which contains the logic of the next middleware in the chain.</param>
        /// <param name="options">An options accessor to get access to the tracing configuration.</param>
        public TracingMiddleware(RequestDelegate next,
            IOptions<TracingMiddlewareConfiguration> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next;
            _configuration = options.Value;
        }

        /// <summary>
        /// Invokes the middleware logic.
        /// </summary>
        /// <param name="context">A <c>HTTP</c> context</param>
        public async Task InvokeAsync(HttpContext context)
        {
            // todo: implement!

            if (_next != null)
            {
                await _next(context).ConfigureAwait(false);
            }
        }
    }
}