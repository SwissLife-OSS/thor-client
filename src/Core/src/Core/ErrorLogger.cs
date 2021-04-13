using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Thor.Core.Transmission.EventHub
{
    /// <summary>
    /// Creates a log entry limited per interval
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ErrorLogger<T>
    {
        private readonly ILogger<T> _logger;
        private readonly string _instance;

        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);
        private readonly Stopwatch _errorWatch;
        private int _errorsPerInterval = 1;
        private int _errorCount;

        /// <summary>
        /// The watcher ctor
        /// </summary>
        public ErrorLogger(ILogger<T> logger)
        {
            _logger = logger;
            _errorWatch = new Stopwatch();
            _instance = typeof(T).Name;
        }

        /// <summary>
        /// Create a error log entry if interval elapsed or number of errors per interval not exceeded
        /// </summary>
        /// <param name="exception"></param>
        public void Log(Exception exception)
        {
            if (_errorCount < _errorsPerInterval)
            {
                _logger.LogError(exception, $"{_instance} failed");
                Interlocked.Increment(ref _errorCount);
            }

            if (!_errorWatch.IsRunning)
            {
                _errorWatch.Restart();
            }

            if (_errorWatch.Elapsed > _interval)
            {
                Interlocked.Exchange(ref _errorCount, 0);
                _errorWatch.Restart();
            }
        }
    }
}
