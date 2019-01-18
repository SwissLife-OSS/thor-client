using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using Thor.Core.Abstractions;

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

        public static HotChocolateActivity Create(HotChocolateRequest request)
        {
            HotChocolateActivity context = new HotChocolateActivity();

            if (context._relatedActivityId != Guid.Empty)
            {
                HotChocolateActivityEventSource.Log.BeginTransfer(context._relatedActivityId);
                HotChocolateActivityEventSource.Log.Start(context.Id, request);
                HotChocolateActivityEventSource.Log.EndTransfer(context.Id, context._relatedActivityId);
            }
            else
            {
                HotChocolateActivityEventSource.Log.Start(context.Id, request);
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

            HotChocolateActivityEventSource.Log.OnQueryError(exception);
        }

        /// <summary>
        /// Handles query validation error.
        /// </summary>
        /// <param name="errors">Validation errors.</param>
        public void HandleValidationError(IReadOnlyCollection<IError> errors)
        {
            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            if (errors.Count > 0)
            {
                HotChocolateActivityEventSource.Log.OnValidationError(errors
                    .Select(e =>
                        new HotChocolateError
                        {
                            Message = e.Message,
                            Code = e.Code
                        }));
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
                    HotChocolateActivityEventSource.Log.Stop(Id);

                    _popWhenDispose?.Dispose();
                    _popWhenDispose = null;
                }

                _disposed = true;
            }
        }
    }
}