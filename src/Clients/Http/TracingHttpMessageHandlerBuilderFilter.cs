using System;
using Microsoft.Extensions.Http;

namespace Thor.Core.Http
{
    internal class TracingHttpMessageHandlerBuilderFilter
        : IHttpMessageHandlerBuilderFilter
    {
        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            return (builder) =>
            {
                next(builder);
                builder.AdditionalHandlers.Insert(0, new TracingHttpMessageHandler());
            };
        }
    }
}