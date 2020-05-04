using System.Diagnostics.Tracing;
using Thor.Core.Abstractions;

namespace Thor.Hosting.AspNetCore.FunctionalTest.EventSources
{
    [EventSourceDefinition(Name = "AspNetCore-FunctionalTest")]
    public interface IValuesEventSources
    {
        [Event(1,
            Level = EventLevel.Informational,
            Message = "Retrieve value for object {id}",
            Version = 1)]
        void RetrieveObject(int id);
    }
}
