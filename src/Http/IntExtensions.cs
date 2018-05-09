using System;
using System.Net;

namespace Thor.Core.Http
{
    internal static class IntExtensions
    {
        public static string GetHttpStatusText(this int statusCode)
        {
            const string unknown = "UNKNOWN";

            return (statusCode > 0)
                ? Enum.GetName(typeof(HttpStatusCode), statusCode)?.ToUpper() ?? unknown
                : unknown;
        }
    }
}