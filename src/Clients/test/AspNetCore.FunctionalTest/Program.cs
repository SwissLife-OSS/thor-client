using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Thor.Hosting.AspNetCore.FunctionalTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(c => c
                    .ClearProviders()
                    .AddConsole().AddFilter((provider, category, logLevel) =>
                    {
                        if (category.Contains("Thor"))
                        {
                            return true;
                        }

                        if (logLevel > LogLevel.Information)
                        {
                            return true;
                        }

                        return false;
                    }))
                .UseStartup<Startup>()
                .Build();
    }
}
