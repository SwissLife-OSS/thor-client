using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Thor.Core.Abstractions;
using Thor.Core.Session.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace Thor.Core.Session
{
    /// <summary>
    /// An <c>ETW</c> <c>in-process</c> telemetry session to listen to events.
    /// The telemetry session will automatically enable event providers (Event
    /// sources) starting with a certain name (Thor.Core). Custom event
    /// provider can be enabled by invoking the <see cref="EnableProvider"/>
    /// method with a specific name. However, custom event provider must exist
    /// in one of the referenced assemblies.
    /// </summary>
    public class InProcessTelemetrySession
        : EventListener
            , ITelemetrySession
    {
        private readonly ImmutableHashSet<string> _allowedPrefixes;
        private static readonly Type _attributeType =
            typeof(EventSourceAttribute);
        private static readonly Type _baseType = typeof(EventSource);
        private readonly object _lock = new object();
        private ImmutableHashSet<string> _providers =
            ImmutableHashSet.Create<string>();
        private ImmutableHashSet<ITelemetryEventTransmitter> _transmitters =
            ImmutableHashSet.Create<ITelemetryEventTransmitter>();
        private readonly EventLevel _level;
        private readonly AppDomain _currentDomain;
        private readonly string _sessionName;

        private InProcessTelemetrySession(int applicationId, EventLevel level,
            IEnumerable<IProvidersDescriptor> providersDescriptors)
        {
            if (applicationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(applicationId),
                    ExceptionMessages.ApplicationIdMustBeGreaterZero);
            }

            _allowedPrefixes = ImmutableHashSet.Create("Thor.Core");
            _currentDomain = AppDomain.CurrentDomain;
            _sessionName = SessionNameProvider.Create(applicationId);
            _level = level;

            if (providersDescriptors != null)
            {
                foreach (var prefix in providersDescriptors.SelectMany(p => p.Assemblies))
                {
                    _allowedPrefixes = _allowedPrefixes.Add(prefix);
                }
            }
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
        public void Attach(ITelemetryEventTransmitter transmitter)
        {
            if (transmitter == null)
            {
                throw new ArgumentNullException(nameof(transmitter));
            }

            lock (_lock)
            {
                if (!_transmitters.Contains(transmitter))
                {
                    _transmitters = _transmitters.Add(transmitter);
                }
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

                foreach (ITelemetryEventTransmitter transmitter in _transmitters)
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

        private static Dictionary<string, Type> CreateProviderNameMap(
            Type[] eventSourceTypes)
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

        private static Dictionary<string, Type> FindSuitableProviders(
            Assembly assembly)
        {
            Type[] eventSourceTypes = GetLoadableTypes(assembly)
                .Where(IsAssignableFrom)
                .ToArray();

            return CreateProviderNameMap(eventSourceTypes);
        }

        private void FindAndActivateProviders()
        {
            foreach (Assembly assembly in _currentDomain.GetAssemblies())
            {
                VerifyAndActivateProviders(assembly);
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
                VerifyAndActivateProviders(args.LoadedAssembly);
        }

        private void VerifyAndActivateProviders(Assembly assembly)
        {
            foreach (var prefix in _allowedPrefixes)
            {
                if (assembly.FullName.StartsWith(prefix,
                    StringComparison.Ordinal))
                {
                    TryActivateProviders(assembly);
                    break;
                }
            }
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

        private void TryActivateProviderCore(string name, Type type,
            EventLevel level)
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
                        _providers = _providers.Add(name);

                        ProviderActivationEventSource.Log.Activated(name);
                    }
                }
            }
        }

        private void TryActivateProviders(Assembly assembly)
        {
            Dictionary<string, Type> eventSourceTypes = FindSuitableProviders(
                assembly);

            foreach (KeyValuePair<string, Type> info in eventSourceTypes)
            {
                TryActivateProviderCore(info.Key, eventSourceTypes[info.Key],
                    _level);
            }
        }

        private EventSource TryGetProviderIntsance(Type type)
        {
            if (IsAssignableFrom(type))
            {
                const string name = "Log";
                PropertyInfo property = type.GetProperty(name,
                    BindingFlags.Public | BindingFlags.Static);

                if (property != null &&
                    property.PropertyType.IsAssignableFrom(type))
                {
                    return (EventSource)property.GetValue(null);
                }

                FieldInfo field = type.GetField(name,
                    BindingFlags.Public | BindingFlags.Static);

                if (field != null && field.FieldType == type)
                {
                    return (EventSource)type.InvokeMember(name,
                        BindingFlags.GetField, null, null, null,
                        CultureInfo.InvariantCulture);
                }

                ConstructorInfo constructor = type.GetConstructor(
                    BindingFlags.Public, null, Type.EmptyTypes, null);

                if (constructor != null)
                {
                    return (EventSource)constructor.Invoke(null);
                }
            }

            return null;
        }

        #region Static Factory Methods

        /// <summary>
        /// Creates a new instance of <see cref="InProcessTelemetrySession"/>
        /// with severity <see cref="EventLevel.Informational"/>.
        /// </summary>
        /// <param name="applicationId">An unique application id.</param>
        /// <returns>
        /// A new instance of <see cref="InProcessTelemetrySession"/>.
        /// </returns>
        public static InProcessTelemetrySession Create(int applicationId)
        {
            return Create(applicationId, EventLevel.Informational);
        }

        /// <summary>
        /// Creates a new instance of <see cref="InProcessTelemetrySession"/>.
        /// </summary>
        /// <param name="applicationId">An unique application id.</param>
        /// <param name="level">A level of severity.</param>
        /// <returns>
        /// A new instance of <see cref="InProcessTelemetrySession"/>.
        /// </returns>
        public static InProcessTelemetrySession Create(int applicationId,
            EventLevel level)
        {
            return Create(applicationId, level, Enumerable.Empty<IProvidersDescriptor>());
        }

        /// <summary>
        /// Creates a new instance of <see cref="InProcessTelemetrySession"/>.
        /// </summary>
        /// <param name="applicationId">An unique application id.</param>
        /// <param name="level">A level of severity.</param>
        /// <param name="providersDescriptors">
        /// A collection of provider descriptors.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="InProcessTelemetrySession"/>.
        /// </returns>
        public static InProcessTelemetrySession Create(int applicationId,
            EventLevel level, IEnumerable<IProvidersDescriptor> providersDescriptors)
        {
            InProcessTelemetrySession session = new InProcessTelemetrySession(
                applicationId, level, providersDescriptors);

            session.FindAndActivateProviders();
            session.RegisterAssemblyLoadCallback();

            return session;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InProcessTelemetrySession"/>.
        /// </summary>
        /// <param name="configuration">
        /// A session configuration instance.
        /// </param>
        /// <param name="providersDescriptors">
        /// A collection of provider descriptors.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="InProcessTelemetrySession"/>.
        /// </returns>
        public static InProcessTelemetrySession Create(
            SessionConfiguration configuration,
            IEnumerable<IProvidersDescriptor> providersDescriptors)
        {
            return Create(configuration.ApplicationId, configuration.Level,
                providersDescriptors);
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