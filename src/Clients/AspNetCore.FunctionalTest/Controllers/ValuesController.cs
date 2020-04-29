using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using static Thor.Hosting.AspNetCore.FunctionalTest.EventSources.ValuesEventSources;

namespace Thor.Hosting.AspNetCore.FunctionalTest.Controllers
{
    [Route("[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet("{count}")]
        public string Get(int count)
        {
            var timer = Stopwatch.StartNew();
            var processed = 0;
            for (processed = 0; processed < count; processed++)
            {
                Log.RetrieveObject(processed);
            }

            return $"{processed} {timer.Elapsed}";
        }
    }
}
