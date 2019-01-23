using System.Diagnostics.Tracing;
using Thor.Core.Abstractions;

namespace Thor.Core.Session
{
    [EventSource(Name = EventSourceNames.ProviderActivation)]
    internal sealed class ProviderActivationEventSource
        : EventSourceBase
    {
        public static ProviderActivationEventSource Log { get; } = new ProviderActivationEventSource();

        private ProviderActivationEventSource() { }

        #region Activated

        [Event(1, Level = EventLevel.Verbose, Message = "Activated provider \"{0}\"", Version = 1)]
        public void Activated(string name)
        {
            if (IsEnabled())
            {
                WriteEvent(1, name);
            }
        }

        #endregion

        #region Activating

        [Event(2, Level = EventLevel.Verbose, Message = "Activating provider \"{0}\" ...", Version = 1)]
        public void Activating(string name)
        {
            if (IsEnabled())
            {
                WriteEvent(2, name);
            }
        }

        #endregion

        #region AlreadyActivated

        [Event(3, Level = EventLevel.Verbose, Message = "Provider \"{0}\" already activated", Version = 1)]
        public void AlreadyActivated(string name)
        {
            if (IsEnabled())
            {
                WriteEvent(3, name);
            }
        }

        #endregion

        #region NoInstance

        [Event(4, Level = EventLevel.Verbose, Message = "Provider \"{0}\" could not be instantiated", Version = 1)]
        public void NoInstance(string name)
        {
            if (IsEnabled())
            {
                WriteEvent(4, name);
            }
        }

        #endregion

        #region NotFound

        [Event(5, Level = EventLevel.Verbose, Message = "Provider \"{0}\" not found", Version = 1)]
        public void NotFound(string name)
        {
            if (IsEnabled())
            {
                WriteEvent(5, name);
            }
        }

        #endregion
    }
}