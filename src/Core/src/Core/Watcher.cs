using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// Creates a log entry on every checkpoint
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Watcher<T>
    {
        private readonly TimeSpan _interval = TimeSpan.FromHours(1);
        private readonly ILogger<T> _logger;
        private readonly Stopwatch _watch;
        private readonly string _instance;

        /// <summary>
        /// The watcher ctor
        /// </summary>
        public Watcher(ILogger<T> logger)
        {
            _logger = logger;
            _watch = Stopwatch.StartNew();
            _instance = typeof(T).Name;
        }

        /// <summary>
        /// Creates a log entry if interval elapsed
        /// </summary>
        public void Checkpoint()
        {
            if (_watch.Elapsed > _interval)
            {
                _logger.LogInformation("Checkpoint from {_instance}", _instance);
                _watch.Restart();
            }
        }
    }
}
