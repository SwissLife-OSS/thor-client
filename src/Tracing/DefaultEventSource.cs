using ChilliCream.Tracing.Abstractions;
using System;
using System.Diagnostics.Contracts;
using System.Diagnostics.Tracing;
using System.Globalization;

namespace ChilliCream.Tracing
{
    /// <summary>
    /// A default <see cref="EventSource"/> in case semantic logging is not favoured but the power of ETW.
    /// This <see cref="EventSource"/> is also helpful when starting with semantic logging in existing project step-by-step.
    /// </summary>
    [EventSource(Name = EventSourceNames.Default)]
    public sealed class DefaultEventSource
        : EventSourceBase
    {
        private const int _criticalEventId = 1;
        private const int _errorEventId = 2;
        private const int _infoEventId = 3;
        private const int _verboseEventId = 4;
        private const int _warningEventId = 5;

        /// <summary>
        /// A static instance of the <see cref="DefaultEventSource"/> class.
        /// </summary>
        public static readonly DefaultEventSource Log = new DefaultEventSource();

        private DefaultEventSource() { }

        #region Critical

        /// <summary>
        /// Writes a critical error message to the logger's event source.
        /// Use critical errors for fatal exception.
        /// </summary>
        /// <param name="message">An arbitrary message.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="message"/> is empty, null or contains only whitespaces.
        /// </exception>
        [NonEvent]
        public void Critical(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(nameof(message));
            }
            Contract.EndContractBlock();

            if (IsEnabled())
            {
                Critical(Application.Id, ActivityStack.Id, message);
            }
        }

        /// <summary>
        /// Writes a critical error message to the logger's event source.
        /// </summary>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more format items, 
        /// which correspond to objects in the <paramref name="args"/> array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="format"/> is empty, null or contains only whitespaces.
        /// </exception>
        [NonEvent]
        public void Critical(string format, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentException(nameof(format));
            }
            Contract.EndContractBlock();

            if (IsEnabled())
            {
                Critical(string.Format(CultureInfo.InvariantCulture, format, args));
            }
        }

        [Event(_criticalEventId, Level = EventLevel.Critical, Message = "{2}", Version = 1)]
        private void Critical(int applicationId, Guid activityId, string message) =>
            WriteCore(_criticalEventId, applicationId, activityId,
                message.TruncateIfExceedLength());

        #endregion

        #region Error

        /// <summary>
        /// Writes an error message to the logger's event source.
        /// Use an error for any exception.
        /// </summary>
        /// <param name="message">An arbitrary message.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="message"/> is empty, null or contains only whitespaces.
        /// </exception>
        [NonEvent]
        public void Error(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(nameof(message));
            }
            Contract.EndContractBlock();

            if (IsEnabled())
            {
                Error(Application.Id, ActivityStack.Id, message);
            }
        }

        /// <summary>
        /// Writes an error message to the logger's event source.
        /// </summary>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more format items, 
        /// which correspond to objects in the <paramref name="args"/> array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="format"/> is empty, null or contains only whitespaces.
        /// </exception>
        [NonEvent]
        public void Error(string format, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentException(nameof(format));
            }
            Contract.EndContractBlock();

            if (IsEnabled())
            {
                Error(string.Format(CultureInfo.InvariantCulture, format, args));
            }
        }

        [Event(_errorEventId, Level = EventLevel.Error, Message = "{2}", Version = 1)]
        private void Error(int applicationId, Guid activityId, string message) =>
            WriteCore(_errorEventId, applicationId, activityId,
                message.TruncateIfExceedLength());

        #endregion

        #region Info

        /// <summary>
        /// Writes an information message.
        /// Informational is usefull basic monitoring.
        /// </summary>
        /// <param name="message">An arbitrary message.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="message"/> is empty, null or contains only whitespaces.
        /// </exception>
        [NonEvent]
        public void Info(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(nameof(message));
            }
            Contract.EndContractBlock();

            if (IsEnabled())
            {
                Info(Application.Id, ActivityStack.Id, message);
            }
        }

        /// <summary>
        /// Writes an information message to the logger's event source.
        /// </summary>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more format items, 
        /// which correspond to objects in the <paramref name="args"/> array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="format"/> is empty, null or contains only whitespaces.
        /// </exception>
        [NonEvent]
        public void Info(string format, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentException(nameof(format));
            }
            Contract.EndContractBlock();

            if (IsEnabled())
            {
                Info(string.Format(CultureInfo.InvariantCulture, format, args));
            }
        }

        [Event(_infoEventId, Level = EventLevel.Informational, Message = "{2}", Version = 1)]
        private void Info(int applicationId, Guid activityId, string message) =>
            WriteCore(_infoEventId, applicationId, activityId,
                message.TruncateIfExceedLength());

        #endregion

        #region Verbose

        /// <summary>
        /// Writes a verbose message. 
        /// Verbose is usefull for advanced monitoring scenarios.
        /// </summary>
        /// <param name="message">An arbitrary message.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="message"/> is empty, null or contains only whitespaces.
        /// </exception>
        [NonEvent]
        public void Verbose(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(nameof(message));
            }
            Contract.EndContractBlock();

            if (IsEnabled())
            {
                Verbose(Application.Id, ActivityStack.Id, message);
            }
        }

        /// <summary>
        /// Writes a verbose message.
        /// </summary>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more format items, 
        /// which correspond to objects in the <paramref name="args"/> array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="format"/> is empty, null or contains only whitespaces.
        /// </exception>
        [NonEvent]
        public void Verbose(string format, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentException(nameof(format));
            }
            Contract.EndContractBlock();

            if (IsEnabled())
            {
                Verbose(string.Format(CultureInfo.InvariantCulture, format, args));
            }
        }

        [Event(_verboseEventId, Level = EventLevel.Verbose, Message = "{2}", Version = 1)]
        private void Verbose(int applicationId, Guid activityId, string message) =>
            WriteCore(_verboseEventId, applicationId, activityId,
                message.TruncateIfExceedLength());

        #endregion

        #region Warning

        /// <summary>
        /// Writes a warning message.
        /// Warnings are usefull to tell someone for example that a limit is reached.
        /// </summary>
        /// <param name="message">An arbitrary message.</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="message"/> is empty, null or contains only whitespaces.
        /// </exception>
        [NonEvent]
        public void Warning(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(nameof(message));
            }
            Contract.EndContractBlock();

            if (IsEnabled())
            {
                Warning(Application.Id, ActivityStack.Id, message);
            }
        }

        /// <summary>
        /// Writes a warning message.
        /// </summary>
        /// <param name="format">
        /// A composite format string that contains text intermixed with zero or more format items, 
        /// which correspond to objects in the <paramref name="args"/> array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws if <paramref name="format"/> is empty, null or contains only whitespaces.
        /// </exception>
        [NonEvent]
        public void Warning(string format, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                throw new ArgumentException(nameof(format));
            }
            Contract.EndContractBlock();

            if (IsEnabled())
            {
                Warning(string.Format(CultureInfo.InvariantCulture, format, args));
            }
        }

        [Event(_warningEventId, Level = EventLevel.Warning, Message = "{2}", Version = 1)]
        private void Warning(int applicationId, Guid activityId, string message) =>
            WriteCore(_warningEventId, applicationId, activityId,
                message.TruncateIfExceedLength());

        #endregion
    }
}