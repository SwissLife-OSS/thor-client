using System;
using Thor.Core.Abstractions;
using static Thor.Core.Http.RequestEventSource;

namespace Thor.Core.Http
{
    /// <summary>
    /// A server-side HTTP request scope to ensures event grouped across threads and processes.
    /// </summary>
    [Serializable]
    public class ServerRequestScope
        : IActivity
    {
        private readonly Guid _activityId = Guid.NewGuid();
        private readonly Guid _relatedActivityId;
        private bool _disposed;
        private HttpResponse _httpResponse;
        private IDisposable _popWhenDispose;
        private int? _statusCode;
        private Guid? _userId;

        private ServerRequestScope()
        {
            _relatedActivityId = ActivityStack.Id;
            _popWhenDispose = ActivityStack.Push(_activityId);
        }

        /// <inheritdoc />
        public Guid Id => _activityId;

        /// <summary>
        /// Creates a server-side HTTP request scope.
        /// </summary>
        /// <param name="method">A HTTP method of a request.</param>
        /// <param name="uri">A HTTP uri of a request.</param>
        /// <param name="relatedActivityId">
        /// A related activity identifier to link a activity scope across component or service 
        /// boundaries with a related scope. Do not use it for initialize a new activity scope!
        /// </param>
        /// <returns>A new instance of <see cref="ServerRequestScope"/>.</returns>
        /// <remarks>
        /// This method is more or less for internal use. Use instead the <c>Thor.AspNetCore</c> package.
        /// </remarks>
        public static ServerRequestScope Create(string method, Uri uri, Guid? relatedActivityId)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            ServerRequestScope context = new ServerRequestScope();

            if (context._relatedActivityId != Guid.Empty)
            {
                Log.OuterScopeNotAllowed(context.Id);
            }

            if (relatedActivityId != null && relatedActivityId != Guid.Empty)
            {
                Log.BeginTransfer(relatedActivityId.Value);
                Log.Start(context._activityId, method, uri.ToString());
                Log.EndTransfer(context._activityId, relatedActivityId.Value);
            }
            else
            {
                Log.Start(context._activityId, method, uri.ToString());
            }

            return context;
        }

        /// <summary>
        /// Creates a server-side HTTP request scope.
        /// </summary>
        /// <param name="request">A HTTP request.</param>
        /// <param name="relatedActivityId">
        /// A related activity identifier to link a activity scope across component or service 
        /// boundaries with a related scope. Do not use it for initialize a new activity scope!
        /// </param>
        /// <returns>A new instance of <see cref="ServerRequestScope"/>.</returns>
        /// <remarks>
        /// This method is more or less for internal use. Use instead the <c>Thor.AspNetCore</c> package.
        /// </remarks>
        public static ServerRequestScope Create(HttpRequest request, Guid? relatedActivityId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ServerRequestScope context = new ServerRequestScope();

            if (context._relatedActivityId != Guid.Empty)
            {
                Log.OuterScopeNotAllowed(context.Id);
            }

            if (relatedActivityId != null && relatedActivityId != Guid.Empty)
            {
                Log.BeginTransfer(relatedActivityId.Value);
                Log.Start(context._activityId, request);
                Log.EndTransfer(context._activityId, relatedActivityId.Value);
            }
            else
            {
                Log.Start(context._activityId, request);
            }

            return context;
        }

        /// <summary>
        /// Sets the response information which will be persited in the logging stream.
        /// </summary>
        /// <param name="statusCode">A HTTP response status code.</param>
        /// <param name="userId">An optional user id.</param>
        public void SetResponse(int statusCode, Guid? userId)
        {
            _statusCode = statusCode;
            _userId = userId;
        }

        /// <summary>
        /// Sets the response information object which will be persited in the logging stream.
        /// </summary>
        /// <param name="response">An object that contains details about the HTTP response.</param>
        public void SetResponse(HttpResponse response)
        {
            _httpResponse = response;
        }

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing">A value indicating whether managed resources should be released.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_httpResponse == null)
                    {
                        Log.Stop(_activityId, _userId ?? Guid.Empty, _statusCode ?? 0);
                    }
                    else
                    {
                        Log.Stop(_activityId, _httpResponse);
                    }

                    _popWhenDispose?.Dispose();
                    _popWhenDispose = null;
                }

                _disposed = true;
            }
        }

        #endregion
    }
}