using System;
using System.Diagnostics.Contracts;
using static ChilliCream.Logging.ApplicationEventSource;

namespace ChilliCream.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public static class Application
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        public static int Id { get; private set; }

        /// <summary>
        /// Sends a start signal for application starts to the ETW session.
        /// Therefore the methos <see cref="Start"/> must be called at the very beginning before any other code run.
        /// This method also ensures the start signal is called only once even this method is called twice.
        /// </summary>
        /// <param name="id">An application id that will be connected to every event.</param>
        public static void Start(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id),
                    ExceptionMessages.ApplicationIdMustBeGreaterZero);
            }
            Contract.EndContractBlock();

            if (Id == default(int))
            {
                lock (_lock)
                {
                    if (Id == default(int))
                    {
                        Id = id;
                        Log.Start(id);
                    }
                }
            }
        }

        /// <summary>
        /// Sends a stop signal for application starts to the ETW session.
        /// Therefore the methos <see cref="Stop"/> must be called at the very ending after any other code.
        /// This method also ensures the stop signal is called only once even this method is called twice.
        /// </summary>
        public static void Stop()
        {
            if (Id > 0)
            {
                lock (_lock)
                {
                    if (Id > 0)
                    {
                        Log.Stop(Id);
                        Id = 0;
                    }
                }
            }
        }
    }
}