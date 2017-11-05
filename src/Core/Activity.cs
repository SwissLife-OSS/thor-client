using Thor.Core.Abstractions;
using System;
using System.Runtime.CompilerServices;
using static Thor.Core.ActivityEventSource;

namespace Thor.Core
{
    /// <summary>
    /// A default activity to group log events even across threads.
    /// </summary>
    /// <example>
    /// Here is an Example of creating an activity.
    /// Keep in mind to always await async code inside a <c>using</c> block.
    /// <code>
    /// <![CDATA[
    /// using (Activity.Create("An arbitrary but unique name"))
    /// {
    ///     Log.Error("Error message!");
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <remarks>
    /// Always await asnyc code blocks inside an activity.
    /// Otherwise log events are not grouped correct or sorted in the right order.
    /// </remarks>
    [Serializable]
    public sealed class Activity
        : IActivity
    {
        private readonly Guid _activityId = Guid.NewGuid();
        private readonly Guid _relatedActivityId;
        private readonly string _name;
        private readonly IDisposable _popWhenDisposed;
        private bool _disposed;

        private Activity(string name)
        {
            _name = name;
            _relatedActivityId = ActivityStack.Id;
            _popWhenDisposed = ActivityStack.Push(_activityId);
        }

        /// <inheritdoc />
        public Guid Id => _activityId;

        /// <summary>
        /// Creates a new root or child activity.
        /// </summary>
        /// <param name="callerName">A member caller name provided at compile-time.</param>
        /// <returns>A new instance of <see cref="Activity"/>.</returns>
        public static Activity CreateWithCallerName([CallerMemberName]string callerName = "") =>
            Create($"Method: {callerName}");

        /// <summary>
        /// Creates a new root or child activity.
        /// </summary>
        /// <param name="callerName">A member caller name provided at compile-time.</param>
        /// <param name="callerFilePath">A member caller file path provided at compile-time.</param>
        /// <param name="callerLineNumber">A member caller line number provided at compile-time.</param>
        /// <returns>A new instance of <see cref="Activity"/>.</returns>
        public static Activity CreateWithCallerInfo([CallerMemberName]string callerName = "", 
            [CallerFilePath]string callerFilePath = "", [CallerLineNumber]int callerLineNumber = 0) =>
                Create($"Method: {callerName} in {callerFilePath} at line number {callerLineNumber}");

        /// <summary>
        /// Creates a new root or child activity.
        /// </summary>
        /// <param name="name">A name to help understanding logs. A name should be unique to avoid obscurities.</param>
        /// <returns>A new instance of <see cref="Activity"/>.</returns>
        public static Activity Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Activity activity = new Activity(name);

            if (activity._relatedActivityId == Guid.Empty)
            {
                Log.Start(activity.Id, name);
            }
            else
            {
                Log.BeginTransfer(activity._relatedActivityId);
                Log.Start(activity.Id, name);
                Log.EndTransfer(activity.Id, activity._relatedActivityId);
            }

            return activity;
        }

        #region Dispose

        /// <inheritdoc />
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Releases resources held by the activity.
        /// </summary>
        /// <param name="disposing">A value indicating whether the resources should be released.</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Log.Stop(_activityId, _name);
                    _popWhenDisposed?.Dispose();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}