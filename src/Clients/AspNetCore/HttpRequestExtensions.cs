using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Thor.Core.Abstractions;

namespace Thor.AspNetCore
{
    internal static class HttpRequestExtensions
    {
        public static Guid? GetActivityId(this HttpRequest request)
        {
            if (request != null &&
                request.Headers.TryGetValue(MessageHeaderKeys.ActivityId, out StringValues rawActivityIds) &&
                rawActivityIds.Count > 0 &&
                Guid.TryParse(rawActivityIds[0], out Guid id) &&
                id != Guid.Empty)
            {
                return id;
            }

            return null;
        }
    }
}