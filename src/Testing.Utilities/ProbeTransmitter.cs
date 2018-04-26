using System.Collections.Concurrent;
using Thor.Core.Abstractions;

namespace Thor.Core.Testing.Utilities
{
    public class ProbeTransmitter
        : ITelemetryTransmitter
    {
        private readonly ConcurrentQueue<TelemetryEvent> _queue = new ConcurrentQueue<TelemetryEvent>();

        public int Count { get { return _queue.Count; } }

        public void Dispose() { }

        public void Enqueue(TelemetryEvent telemetryEvent)
        {
            _queue.Enqueue(telemetryEvent);
        }

        public bool TryDequeue(out TelemetryEvent telemetryEvent)
        {
            return _queue.TryDequeue(out telemetryEvent);
        }
    }
}