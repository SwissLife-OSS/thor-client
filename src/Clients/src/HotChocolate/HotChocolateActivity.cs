using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Execution.Instrumentation;
using Thor.Core.Abstractions;
using static Thor.Extensions.HotChocolate.HotChocolateActivityEventSource;

namespace Thor.Extensions.HotChocolate
{
    /// <summary>
    /// A server-side HotChocolate request activity to group telemetry events across threads and processes.
    /// </summary>
    [Serializable]
    internal class HotChocolateActivity
        : IActivity
        , IDisposable
    {
        private readonly Guid _relatedActivityId;
        private bool _disposed;
        private IDisposable _popWhenDispose;

        private HotChocolateActivity(HotChocolateRequest request)
        {
            _relatedActivityId = ActivityStack.Id;
            _popWhenDispose = ActivityStack.Push(Id);
            Request = request;
        }

        /// <inheritdoc />
        public Guid Id { get; } = Guid.NewGuid();

        public HotChocolateRequest Request { get; }

        public static HotChocolateActivity Create(HotChocolateRequest request)
        {
            var context = new HotChocolateActivity(request);

            if (context._relatedActivityId != Guid.Empty)
            {
                Log.BeginTransfer(context._relatedActivityId);
                Log.Start(context.Id, request);
                Log.EndTransfer(context.Id, context._relatedActivityId);
            }
            else
            {
                Log.Start(context.Id, request);
            }

            return context;
        }

        /// <summary>
        /// Handles query error.
        /// </summary>
        /// <param name="exception">Query exception.</param>
        public void HandleQueryError(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Log.OnQueryError(exception);
        }

        /// <summary>
        /// Handles resolver error tracing.
        /// </summary>
        /// <param name="request">The request details.</param>
        /// <param name="error">The error details.</param>
        public void HandlesResolverError(
            HotChocolateRequest request,
            IError error)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            HotChocolateError[] formattedErrors =
            {
                new HotChocolateError
                {
                    Message = error.Message,
                    Code = error.Code,
                    Path = error.Path?.ToList(),
                    Exception = error.Exception
                }
            };

            var message = error.Exception == null
                ? error.Message
                : error.Exception.Message;

            Log.OnResolverError(message, request, formattedErrors);
        }

        /// <summary>
        /// Handles validation error tracing.
        /// </summary>
        /// <param name="request">The request details.</param>
        /// <param name="errors">The error details.</param>
        public void HandleValidationErrors(
            HotChocolateRequest request,
            IReadOnlyList<IError> errors)
        {
            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            if (errors.Count > 0)
            {
                var formattedErrors =
                     errors.Select(e =>
                        new HotChocolateError
                        {
                            Message = e.Message,
                            Code = e.Code,
                            Path = e.Path?.ToList(),
                            Exception = e.Exception
                        }).ToList();

                HotChocolateError firstError = formattedErrors[0];

                var message = firstError.Exception == null
                    ? firstError.Message
                    : firstError.Exception.Message;

                Log.OnValidationError(message, request, formattedErrors);
            }
        }

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
                    Log.Stop(Id);

                    _popWhenDispose?.Dispose();
                    _popWhenDispose = null;
                }

                _disposed = true;
            }
        }
    }
}
