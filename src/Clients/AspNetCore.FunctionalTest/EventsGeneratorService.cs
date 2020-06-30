using System;
using System.Threading;
using System.Threading.Tasks;
using Thor.Extensions.Hosting;
using Thor.Hosting.AspNetCore.FunctionalTest.Controllers;

namespace Thor.Hosting.AspNetCore.FunctionalTest
{
    public class EventsGeneratorService : BackgroundServiceBase
    {
        private readonly EventsGenerator _eventsGenerator;

        public EventsGeneratorService(EventsGenerator eventsGenerator)
        {
            _eventsGenerator = eventsGenerator;
        }

        protected override async Task OnExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _eventsGenerator.CreateEvents();
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
