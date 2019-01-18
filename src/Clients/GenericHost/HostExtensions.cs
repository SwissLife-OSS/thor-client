using System;
using Microsoft.Extensions.Hosting;
using Thor.Core;

namespace Thor.GenericHost
{
    internal static class HostExtensions
    {
        internal static void RunSafe(this IHost host)
        {
            try
            {
                host.Run();
            }
            catch (Exception ex)
            {
                Application.UnhandledException(ex);
            }
        }
    }
}
