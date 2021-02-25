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
        private readonly ILogger<T> _logger;
        private readonly string _instance;

        private readonly TimeSpan _interval = TimeSpan.FromHours(1);
        private readonly Stopwatch _checkpointWatch;
        private readonly Stopwatch _errorWatch;
        private int _errorsPerInterval = 1;
        private int _errorCount;
        private static object _errorSync = new object();

        /// <summary>
        /// The watcher ctor
        /// </summary>
        public Watcher(ILogger<T> logger)
        {
            _logger = logger;
            _checkpointWatch = new Stopwatch();
            _errorWatch = new Stopwatch();
            _instance = typeof(T).Name;
        }

        /// <summary>
        /// Creates a log entry if interval elapsed
        /// </summary>
        public void Checkpoint()
        {
            if (!_checkpointWatch.IsRunning || _checkpointWatch.Elapsed > _interval)
            {
                _logger.LogInformation("Checkpoint from {_instance}", _instance);
                _checkpointWatch.Restart();
            }
        }

        /// <summary>
        /// Create a error log entry if interval elapsed or number of errors per interval not exceeded
        /// </summary>
        /// <param name="exception"></param>
        public void ReportError(Exception exception)
        {
            lock (_errorSync)
            {
                if (_errorCount < _errorsPerInterval)
                {
                    _logger.LogError(exception, "{_instance} failed", _instance);
                    _errorCount++;
                }

                if (!_errorWatch.IsRunning)
                {
                    _errorWatch.Restart();
                }

                if (_errorWatch.Elapsed > _interval)
                {
                    _errorCount = 0;
                    _errorWatch.Restart();
                }
            }
        }
    }
}
