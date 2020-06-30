using System.Diagnostics;
using Thor.Hosting.AspNetCore.FunctionalTest.EventSources;

namespace Thor.Hosting.AspNetCore.FunctionalTest.Controllers
{
    public class EventsGenerator
    {
        public string CreateEvents(int count = 1)
        {
            var timer = Stopwatch.StartNew();
            var processed = 0;
            for (processed = 0; processed < count; processed++)
            {
                ValuesEventSources.Log.RetrieveObject(processed);
            }

            return $"{processed} {timer.Elapsed}";
        }

        public string CreateEventsWithAttachment(string id, int count = 1)
        {
            var timer = Stopwatch.StartNew();
            var processed = 0;
            for (processed = 0; processed < count; processed++)
            {
                ValuesEventSources.Log.RetrieveObjectInfo(processed, new RequestInformation { Info = id });
            }

            return $"{processed} {timer.Elapsed}";
        }
    }
}
