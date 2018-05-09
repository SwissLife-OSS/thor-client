using System;
using Thor.Core.Abstractions;
using static Thor.Core.Http.RequestEventSource;

namespace Thor.Core.Http
{
    /// <summary>
    /// A client-side HTTP request scope to ensures event grouped across threads and processes.
    /// </summary>
    [Serializable]
    public class ClientRequestScope
        : IActivity
    {
        private readonly Guid _activityId = Guid.NewGuid();
        private bool _disposed;
        private readonly Guid _relatedActivityId;
        private readonly IDisposable _popWhenDispose;
        private int? _statusCode;
        private Guid? _userId;

        private ClientRequestScope()
        {
            _relatedActivityId = ActivityStack.Id;
            _popWhenDispose = ActivityStack.Push(_activityId);
        }

        /// <inheritdoc />
        public Guid Id => _activityId;

        /// <summary>
        /// Creates a client-side HTTP request scope.
        /// </summary>
        /// <param name="method">A HTTP method of a request.</param>
        /// <param name="uri">A HTTP uri of a request.</param>
        /// <returns>A new instance of <see cref="ClientRequestScope"/>.</returns>
        public static ClientRequestScope Create(string method, Uri uri)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            ClientRequestScope context = new ClientRequestScope();

            if (context._relatedActivityId == Guid.Empty)
            {
                Log.Send(context._activityId, method, uri.ToString());
            }
            else
            {
                Log.BeginTransfer(context._relatedActivityId);
                Log.Send(context._activityId, method, uri.ToString());
                Log.EndTransfer(context._activityId, context._relatedActivityId);
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

        #region Dispose

        /// <inheritdoc />
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing">A value indicating whether the resources should be released.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Log.Receive(_activityId, _userId ?? Guid.Empty, _statusCode ?? 0);
                    _popWhenDispose?.Dispose();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}