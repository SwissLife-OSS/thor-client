using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Thor.Hosting.AspNetCore.FunctionalTest.EventSources;
using static Thor.Hosting.AspNetCore.FunctionalTest.EventSources.ValuesEventSources;

namespace Thor.Hosting.AspNetCore.FunctionalTest.Controllers
{
    public class ValuesController : Controller
    {
        [Route("/events/{count}")]
        public string Events(int count)
        {
            var timer = Stopwatch.StartNew();
            var processed = 0;
            for (processed = 0; processed < count; processed++)
            {
                Log.RetrieveObject(processed);
            }

            return $"{processed} {timer.Elapsed}";
        }

        [Route("/attachments/{count}")]
        public string EventsWithAttachment(int count)
        {
            var timer = Stopwatch.StartNew();
            var processed = 0;
            for (processed = 0; processed < count; processed++)
            {
                Log.RetrieveObjectInfo(processed, new RequestInformation { Info = HttpContext.TraceIdentifier });
            }

            return $"{processed} {timer.Elapsed}";
        }
    }
}
