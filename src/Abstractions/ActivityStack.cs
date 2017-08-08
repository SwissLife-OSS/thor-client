using System;
using System.Threading;

namespace ChilliCream.Logging.Abstractions
{
    /// <summary>
    /// Holds the complete activity state in a stack.
    /// </summary>
    public static class ActivityStack
    {
        private static readonly string _name = Guid.NewGuid().ToString();
        private static readonly AsyncLocal<ActivityState> _callContext = new AsyncLocal<ActivityState>();

        /// <summary>
        /// Gets or sets the actual state.
        /// </summary>
        public static ImmutableStack<Guid> Current
        {
            get
            {
                ActivityState state = _callContext.Value as ActivityState;

                return (state == null) ? ImmutableStack<Guid>.Empty : state.ActivityIds;
            }
            set
            {
                _callContext.Value = new ActivityState { ActivityIds = value };
            }
        }

        /// <summary>
        /// Gets the activity id on the top of the stack; or <see cref="Guid.Empty"/> if the stack is empty.
        /// </summary>
        /// <returns>An activity id on the top of the stack; or <see cref="Guid.Empty"/> if the stack is empty.</returns>
        public static Guid GetTopOrEmpty()
        {
            return (Current.IsEmpty) ? Guid.Empty : Current.Peek();
        }

        /// <summary>
        /// Gets the activity id on the top of the stack.
        /// </summary>
        /// <returns>An activity id on the top of the stack.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the stack is empty.
        /// </exception>
        public static Guid Peek()
        {
            if (Current.IsEmpty)
            {
                throw new InvalidOperationException("The stack is empty; no activity id was found.");
            }

            return Current.Peek();
        }

        private static void Pop()
        {
            Current = Current.Pop();
        }

        /// <summary>
        /// Pushes an activity id onto the stack and returns a <see cref="IDisposable"/> instance.
        /// </summary>
        /// <param name="activityId">An activity id to push onto the stack.</param>
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