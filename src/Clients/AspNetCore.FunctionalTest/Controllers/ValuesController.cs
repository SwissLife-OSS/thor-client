using Microsoft.AspNetCore.Mvc;

namespace Thor.Hosting.AspNetCore.FunctionalTest.Controllers
{
    public class ValuesController : Controller
    {
        private readonly EventsGenerator _eventsGenerator;

        public ValuesController(EventsGenerator eventsGenerator)
        {
            _eventsGenerator = eventsGenerator;
        }

        [Route("/events/{count}")]
        public string Events(int count)
        {
            return _eventsGenerator.CreateEvents(count);
        }

        [Route("/attachments/{count}")]
        public string EventsWithAttachment(int count)
        {
            return _eventsGenerator.CreateEventsWithAttachment(HttpContext.TraceIdentifier, count);
        }
    }
}
