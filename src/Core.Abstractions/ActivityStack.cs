using System;
using System.Threading;

namespace Thor.Core.Abstractions
{
    /// <summary>
    /// A stack that holds the complete activity state.
    /// </summary>
    public static class ActivityStack
    {
        private static readonly AsyncLocal<ActivityState> _callContext = new AsyncLocal<ActivityState>();

        private static ImmutableStack<Guid> Current
        {
            get
            {
                ActivityState state = _callContext.Value;

                return (state == null) ? ImmutableStack<Guid>.Empty : state.ActivityIds;
            }
            set
            {
                _callContext.Value = new ActivityState { ActivityIds = value };
            }
        }

        /// <summary>
        /// Gets the current activity identifier.
        /// </summary>
        public static Guid Id
        {
            get
            {
                return GetTopOrEmpty();
            }
        }

        private static Guid GetTopOrEmpty()
        {
            return (Current.IsEmpty) ? Guid.Empty : Current.Peek();
        }

        private static void Pop()
        {
            Current = Current.Pop();
        }

        /// <summary>
        /// Pushes an activity id to the stack and returns a <see cref="IDisposable"/> instance.
        /// </summary>
        /// <param name="activityId">An activity identifier.</param>
        /// <returns>A <see cref="IDisposable"/> instance which cleans up the state on dispose.</returns>
        public static IDisposable Push(Guid activityId)
        {
            Current = Current.Push(activityId);

            return new PopWhenDisposed();
        }

        private sealed class ActivityState
            : MarshalByRefObject
        {
            public ImmutableStack<Guid> ActivityIds { get; set; }
        }

        private sealed class PopWhenDisposed
            : IDisposable
        {
            private bool _disposed;

            public void Dispose()
            {
                if (!_disposed)
                {
                    Pop();
                    _disposed = true;
                }
            }
        }
    }
}