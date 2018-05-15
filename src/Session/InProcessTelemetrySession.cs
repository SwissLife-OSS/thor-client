using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Thor.Core.Abstractions;

namespace Thor.Core.Session
{
    /// <summary>
    /// An <c>ETW</c> <c>in-process</c> telemetry session to listen to events.
    /// The telemetry session will automatically enable event providers (Event sources) starting
    /// with a certain name (ChilliCream). Custom event provider can be enabled by invoking the
    /// <see cref="EnableProvider"/> method with a specific name. However, custom event provider
    /// must be exist in one of the referenced assemblies.
    /// </summary>
    public class InProcessTelemetrySession
        : EventListener
        , ITelemetrySession
    {
        private const string _assemblyPrefix = "Thor.Core";
        private static readonly Type _attributeType = typeof(EventSourceAttribute);
        private static readonly Type _baseType = typeof(EventSource);
        private readonly object _lock = new object();
        private HashSet<string> _providers = new HashSet<string>();
        private ITelemetryTransmitter[] _transmitters = new ITelemetryTransmitter[0];
        private readonly EventLevel _level;
        private readonly AppDomain _currentDomain;
        private readonly string _sessionName;

        private InProcessTelemetrySession(int applicationId, EventLevel level)
        {
            if (applicationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(applicationId),
                    ExceptionMessages.ApplicationIdMustBeGreaterZero);
            }

            _currentDomain = AppDomain.CurrentDomain;
            _sessionName = SessionNameProvider.Create(applicationId);
            _level = level;
        }

        /// <inheritdoc />
        public void EnableProvider(string name, EventLevel level)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            TryActivateProvider(name, level);
        }

        /// <inheritdoc />
        public void SetTransmitter(ITelemetryTransmitter transmitter)
        {
            if (transmitter == null)
            {
                throw new ArgumentNullException(nameof(transmitter));
            }

            lock (_lock)
            {
                List<ITelemetryTransmitter> newTransmitters = new List<ITelemetryTransmitter>(_transmitters)
                {
                    transmitter
                };

                _transmitters = newTransmitters.ToArray();
            }
        }

        /// <inheritdoc />
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            // note: here we filter out the EventSource manifest events
            if (eventData != null && eventData.EventId != 0 &&
                eventData.EventId != EventIdentifiers.Manifest)
            {
                TelemetryEvent telemetryEvent = eventData.Map(_sessionName);

                foreach (ITelemetryTransmitter transmitter in _transmitters)
                {
                    transmitter.Enqueue(telemetryEvent);
                }
            }
        }

        private static bool IsAssignableFrom(Type type)
        {
            return type.IsClass && type != _baseType && _baseType.IsAssignableFrom(type) &&
                type.CustomAttributes.Count(a => a.AttributeType == _attributeType) == 1 &&
                type.CustomAttributes.First(a => a.AttributeType == _attributeType)
                    .NamedArguments.Count(a => a.MemberName == "Name" &&
                        !string.IsNullOrWhiteSpace((string)a.TypedValue.Value)) == 1;
        }

        private static Dictionary<string, Type> CreateProviderNameMap(Type[] eventSourceTypes)
        {
            Dictionary<string, Type> eventSourceNameMap = eventSourceTypes
                .ToDictionary(k => (string)k.CustomAttributes
                    .Single(a => a.AttributeType == _attributeType).NamedArguments
                    .Single(a => a.MemberName == "Name").TypedValue.Value, v => v);

            return eventSourceNameMap;
        }

        private static Type FindProviderType(Assembly assembly, string name)
        {
            Func<Type, bool> hasName = type =>
                type.CustomAttributes.Count(a => a.AttributeType == _attributeType) == 1 &&
                type.CustomAttributes.First(a => a.AttributeType == _attributeType)
                    .NamedArguments.Count(a => a.MemberName == "Name" &&
                        (string)a.TypedValue.Value == name) == 1;
            Type providerType = GetLoadableTypes(assembly)
                .FirstOrDefault(type => IsAssignableFrom(type) && hasName(type));

            return providerType;
        }

        private static Dictionary<string, Type> FindSuitableProviders(Assembly assembly)
        {
            Type[] eventSourceTypes = GetLoadableTypes(assembly)
                .Where(IsAssignableFrom)
                .ToArray();

            return CreateProviderNameMap(eventSourceTypes);
        }

        private void FindAndActivateProviders()
        {
            IEnumerable<Assembly> assemblies = _currentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith(_assemblyPrefix, StringComparison.Ordinal));

            foreach (Assembly assembly in assemblies)
            {
                TryActivateProviders(assembly);
            }
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        private void RegisterAssemblyLoadCallback()
        {
            _currentDomain.AssemblyLoad += (sender, args) =>
            {
                if (args.LoadedAssembly.FullName.StartsWith(_assemblyPrefix, StringComparison.Ordinal))
                {
                    TryActivateProviders(args.LoadedAssembly);
                }
            };
        }

        private void TryActivateProvider(string name, EventLevel level)
        {
            Type type = null;

            foreach (Assembly assembly in _currentDomain.GetAssemblies())
            {
                type = FindProviderType(assembly, name);

                if (type != null)
                {
                    break;
                }
            }

            if (type == null)
            {
                ProviderActivationEventSource.Log.NotFound(name);
            }
            else
            {
                TryActivateProviderCore(name, type, level);
            }
        }

        private void TryActivateProviderCore(string name, Type type, EventLevel level)
        {
            lock (_lock)
            {
                if (_providers.Contains(name))
                {
                    ProviderActivationEventSource.Log.AlreadyActivated(name);
                }
                else
                {
                    EventSource instance = TryGetProviderIntsance(type);

                    if (instance == null)
                    {
                        ProviderActivationEventSource.Log.NoInstance(name);
                    }
                    else
                    {
                        ProviderActivationEventSource.Log.Activating(name);

                        EnableEvents(instance, level);
                        _providers = new HashSet<string>(_providers) { name };

                        ProviderActivationEventSource.Log.Activated(name);
                    }
                }
            }
        }

        private void TryActivateProviders(Assembly assembly)
        {
            Dictionary<string, Type> eventSourceTypes = FindSuitableProviders(assembly);

            foreach (KeyValuePair<string, Type> info in eventSourceTypes)
            {
                TryActivateProviderCore(info.Key, eventSourceTypes[info.Key], _level);
            }
        }

        private EventSource TryGetProviderIntsance(Type type)
        {
            if (IsAssignableFrom(type))
            {
                const string name = "Log";
                PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Static);

                if (property != null && property.PropertyType == type)
                {
                    return (EventSource)property.GetValue(null);
                }

                FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.Static);

                if (field != null && field.FieldType == type)
                {
                    return (EventSource)type
                        .InvokeMember(name, BindingFlags.GetField, null, null, null,
                            CultureInfo.InvariantCulture);
                }

                ConstructorInfo constructor = type.GetConstructor(BindingFlags.Public, null,
                    Type.EmptyTypes, null);

                if (constructor != null)
                {
                    return (EventSource)constructor.Invoke(null);
                }
            }

            return null;
        }

        #region Static Factory Methods

        /// <summary>
        /// Creates a new instance of <see cref="InProcessTelemetrySession"/> with severity
        /// <see cref="EventLevel.Informational"/>.
        /// </summary>
        /// <param name="applicationId">An unique application id.</param>
        /// <returns>A new instance of <see cref="InProcessTelemetrySession"/>.</returns>
        public static InProcessTelemetrySession Create(int applicationId)
        {
            return Create(applicationId, EventLevel.Informational);
        }

        /// <summary>
        /// Creates a new instance of <see cref="InProcessTelemetrySession"/>.
        /// </summary>
        /// <param name="applicationId">An unique application id.</param>
        /// <param name="level">A level of severity.</param>
        /// <returns>A new instance of <see cref="InProcessTelemetrySession"/>.</returns>
        public static InProcessTelemetrySession Create(int applicationId, EventLevel level)
        {
            InProcessTelemetrySession session = new InProcessTelemetrySession(applicationId, level);

            session.FindAndActivateProviders();
            session.RegisterAssemblyLoadCallback();

            return session;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InProcessTelemetrySession"/>.
        /// </summary>
        /// <param name="configuration">A session configuration instance.</param>
        /// <returns>A new instance of <see cref="InProcessTelemetrySession"/>.</returns>
        public static InProcessTelemetrySession Create(SessionConfiguration configuration)
        {
            return Create(configuration.ApplicationId, configuration.Level);
        }

        #endregion

        #region Dispose

        /// <inheritdoc/>
        public override void Dispose()
        {
            foreach (IDisposable transmitter in _transmitters.OfType<IDisposable>())
            {
                transmitter.Dispose();
            }

            base.Dispose();
        }

        #endregion
    }
}