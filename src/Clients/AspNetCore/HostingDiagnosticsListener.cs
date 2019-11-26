using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DiagnosticAdapter;
using Thor.Core;
using Thor.Extensions.Http;

namespace Thor.Hosting.AspNetCore
{
    internal class HostingDiagnosticsListener
    {
        private readonly Regex _skipRequestFilter;

        public HostingDiagnosticsListener(string skipRequestFilterPattern)
        {
            if (!string.IsNullOrEmpty(skipRequestFilterPattern))
            {
                _skipRequestFilter = new Regex(skipRequestFilterPattern,
                    RegexOptions.Compiled |
                    RegexOptions.CultureInvariant |
                    RegexOptions.IgnoreCase);
            }
        }

        [DiagnosticName("Microsoft.AspNetCore.Diagnostics.HandledException")]
        public void OnDiagnosticsHandledException(HttpContext httpContext, Exception exception)
        {
            HandleException(httpContext, exception);
        }

        [DiagnosticName("Microsoft.AspNetCore.Diagnostics.UnhandledException")]
        public void OnDiagnosticsUnhandledException(HttpContext httpContext, Exception exception)
        {
            HandleException(httpContext, exception);
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.UnhandledException")]
        public void OnHostingException(HttpContext httpContext, Exception exception)
        {
            HandleException(httpContext, exception);
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.HttpRequestIn")]
        public void OnHttpRequestIn()
        {
            // for registration purposes only; do not remove this empty block!
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.HttpRequestIn.Start")]
        public void OnHttpRequestInStart(HttpContext httpContext)
        {
            Uri requestUri = new Uri(httpContext.Request.GetDisplayUrl());
            if (_skipRequestFilter is { } &&
                _skipRequestFilter.IsMatch(requestUri.AbsoluteUri))
            {
                return;
            }

            ServerRequestActivity activity = ServerRequestActivity.Create(httpContext.Request.Method,
                requestUri, httpContext.Request.GetActivityId());

            httpContext.Features.Set(activity);
        }

        [DiagnosticName("Microsoft.AspNetCore.Hosting.HttpRequestIn.Stop")]
        public void OnHttpRequestInStop(HttpContext httpContext)
        {
            ServerRequestActivity activity = httpContext.Features.Get<ServerRequestActivity>();

            if (activity != null)
            {
                activity.SetResponse(httpContext.Response.StatusCode, httpContext.User.GetId());
                activity.Dispose();
            }
        }

        private void HandleException(HttpContext httpContext, Exception exception)
        {
            var activity = httpContext?.Features.Get<ServerRequestActivity>();
            if (activity != null)
            {
                activity.HandleException(exception);
            }
            else
            {
                Application.UnhandledException(exception);
            }
        }
    }
}
