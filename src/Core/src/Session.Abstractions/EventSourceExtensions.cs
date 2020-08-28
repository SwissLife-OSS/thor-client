using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace Thor.Core.Session.Abstractions
{
    /// <summary>
    /// A bunch of convenient <see cref="EventSource"/> extension methods.
    /// </summary>
    public static class EventSourceExtensions
    {
        /// <summary>
        /// Executes code within a new ETW session. The provided <see cref="EventSource"/> will be
        /// enabled for this particular session.
        /// </summary>
        /// <param name="eventSource">An event provider which will be enabled for this ETW session.</param>
        /// <param name="execute">A function to execute code within this ETW session.</param>
        /// <remarks>
        /// Especially good for unit tests. Should not be used in production code.
        /// </remarks>
        public static void Listen(this EventSource eventSource, Action<ProbeEventListener> execute)
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                try
                {
                    listener.EnableEvents(eventSource, EventLevel.Verbose);
                    execute(listener);
                }
                finally
                {
                    listener.DisableEvents(eventSource);
                }
            }
        }

        /// <summary>
        /// Executes code within a new ETW session. The provided <see cref="EventSource"/> will be
        /// enabled for this particular session.
        /// </summary>
        /// <param name="eventSource">An event provider which will be enabled for this ETW session.</param>
        /// <param name="execute">A asynchronous function to execute code within this ETW session.</param>
        /// <remarks>
        /// Especially good for unit tests. Should not be used in production code.
        /// </remarks>
        public static async Task ListenAsync(this EventSource eventSource, Func<ProbeEventListener, Task> execute)
        {
            using (ProbeEventListener listener = new ProbeEventListener())
            {
                try
                {
                    listener.EnableEvents(eventSource, EventLevel.Verbose);
                    await execute(listener);
                }
                catch { }
                finally
                {
                    listener.DisableEvents(eventSource);
                }
            }
        }
    }
}