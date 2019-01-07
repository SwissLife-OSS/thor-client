using System;
using System.Collections.Generic;
using System.Diagnostics;
using Thor.Core.Abstractions;

namespace Thor.Core
{
    /// <summary>
    /// A <see cref="DiagnosticListener"/> initializer for tracing.
    /// </summary>
    internal class DiagnosticsListenerInitializer
        : IDisposable
        , IObserver<DiagnosticListener>
    {
        private readonly IEnumerable<IDiagnosticsListener> _listeners;
        private readonly List<IDisposable> _subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticsListenerInitializer"/> class.
        /// </summary>
        public DiagnosticsListenerInitializer(IEnumerable<IDiagnosticsListener> listeners)
        {
            _listeners = listeners ?? throw new ArgumentNullException(nameof(listeners));
            _subscriptions = new List<IDisposable>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (IDisposable subscription in _subscriptions)
                {
                    subscription.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public void OnCompleted()
        {
            // do nothing
        }

        /// <inheritdoc />
        public void OnError(Exception error)
        {
            // do nothing
        }

        /// <inheritdoc />
        public void OnNext(DiagnosticListener value)
        {
            foreach (IDiagnosticsListener listener in _listeners)
            {
                if (listener.Name == value.Name)
                {
                    _subscriptions.Add(value.SubscribeWithAdapter(listener));
                }
            }
        }

        /// <summary>
        /// Subscribes the listener to all listeners.
        /// </summary>
        public void Start()
        {
            _subscriptions.Add(DiagnosticListener.AllListeners.Subscribe(this));
        }
    }
}