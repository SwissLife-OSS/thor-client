using System;
using Thor.Core.Abstractions;
using static Thor.Core.Http.RequestActivityEventSource;

namespace Thor.Core.Http
{
    /// <summary>
    /// A server-side HTTP request activity to group telemetry events across threads and processes.
    /// </summary>
    [Serializable]
    public class ServerRequestActivity
        : IActivity
    {
        private readonly Guid _activityId = Guid.NewGuid();
        private readonly Guid _relatedActivityId;
        private bool _disposed;
        private HttpResponse _httpResponse;
        private IDisposable _popWhenDispose;
        private int? _statusCode;
        private Guid? _userId;

        private ServerRequestActivity()
        {
            _relatedActivityId = ActivityStack.Id;
            _popWhenDispose = ActivityStack.Push(_activityId);
        }

        /// <inheritdoc />
        public Guid Id => _activityId;

        /// <summary>
        /// Creates a server-side HTTP request activity.
        /// </summary>
        /// <param name="method">A HTTP method of a request.</param>
        /// <param name="uri">A HTTP uri of a request.</param>
        /// <param name="relatedActivityId">
        /// A related activity identifier to link activities across component or service 
        /// boundaries with a parent activity.
        /// </param>
        /// <returns>A new instance of <see cref="ServerRequestActivity"/>.</returns>
        /// <remarks>
        /// This method is more or less for internal use. Use instead the <c>Thor.AspNetCore</c> package.
        /// </remarks>
        public static ServerRequestActivity Create(string method, Uri uri, Guid? relatedActivityId)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            ServerRequestActivity context = new ServerRequestActivity();

            if (context._relatedActivityId != Guid.Empty)
            {
                Log.OuterActivityNotAllowed(context.Id);
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
        /// Creates a server-side HTTP request activity.
        /// </summary>
        /// <param name="request">A HTTP request.</param>
        /// <param name="relatedActivityId">
        /// A related activity identifier to link activities across component or service 
        /// boundaries with a parent activity.
        /// </param>
        /// <returns>A new instance of <see cref="ServerRequestActivity"/>.</returns>
        /// <remarks>
        /// This method is more or less for internal use. Use instead the <c>Thor.AspNetCore</c> package.
        /// </remarks>
        public static ServerRequestActivity Create(HttpRequest request, Guid? relatedActivityId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ServerRequestActivity context = new ServerRequestActivity();

            if (context._relatedActivityId != Guid.Empty)
            {
                Log.OuterActivityNotAllowed(context.Id);
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
        /// Handles unhandled exception.
        /// </summary>
        /// <param name="exception">An exception.</param>
        public void HandleException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Log.InternalServerErrorOccurred(exception);
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
            _httpResponse = response ?? throw new ArgumentNullException(nameof(response));
        }

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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