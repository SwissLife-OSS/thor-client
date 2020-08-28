using System;

namespace Thor.Core.Session
{
    internal static class SessionNameProvider
    {
        public static string Create()
        {
            return $"Session-{Environment.MachineName}";
        }

        public static string Create(int applicationId)
        {
            return $"Session-{Environment.MachineName}-{applicationId}";
        }
    }
}