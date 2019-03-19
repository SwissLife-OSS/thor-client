using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
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
    {
        private readonly Guid _relatedActivityId;
        private bool _disposed;
        private IDisposable _popWhenDispose;

        private HotChocolateActivity()
        {
            _relatedActivityId = ActivityStack.Id;
            _popWhenDispose = ActivityStack.Push(Id);
        }

        /// <inheritdoc />
        public Guid Id { get; } = Guid.NewGuid();

        public static HotChocolateActivity Create(
            HotChocolateRequest request)
        {
            HotChocolateActivity context = new HotChocolateActivity();

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
        /// <param name="errors">The error details.</param>
        public void HandlesResolverErrors(
            HotChocolateRequest request,
            IReadOnlyCollection<IError> errors)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            if (errors.Count > 0)
            {
                List<HotChocolateError> formattedErrors =
                     errors.Select(e =>
                        new HotChocolateError
                        {
                            Message = e.Message,
                            Code = e.Code,
                            Path = e.Path,
                            Exception = e.Exception
                        }).ToList();

                HotChocolateError firstError = formattedErrors[0];

                string message = firstError.Exception == null
                    ? firstError.Message
                    : firstError.Exception.Message;

                Log.OnResolverError(message, request, formattedErrors);
            }
        }

        /// <summary>
        /// Handles validation error tracing.
        /// </summary>
        /// <param name="request">The request details.</param>
        /// <param name="errors">The error details.</param>
        public void HandleValidationError(
            HotChocolateRequest request,
            IReadOnlyCollection<IError> errors)
        {
            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            if (errors.Count > 0)
            {
                List<HotChocolateError> formattedErrors =
                     errors.Select(e =>
                        new HotChocolateError
                        {
                            Message = e.Message,
                            Code = e.Code,
                            Path = e.Path,
                            Exception = e.Exception
                        }).ToList();

                HotChocolateError firstError = formattedErrors[0];

                string message = firstError.Exception == null
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