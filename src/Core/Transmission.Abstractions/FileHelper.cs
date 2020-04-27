using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    internal static class FileHelper
    {
        public static Task<byte[]> ReadAllBytesAsync(
            string fileName,
            CancellationToken cancellationToken)
        {
#if NETSTANDARD2_0
            return Task.Run(() => File.ReadAllBytes(fileName));
#else
            return File.ReadAllBytesAsync(fileName, cancellationToken);
#endif
        }

        public static Task WriteAllBytesAsync(
            string fileName,
            byte[] bytes,
            CancellationToken cancellationToken)
        {
#if NETSTANDARD2_0
            return Task.Run(() => File.WriteAllBytes(fileName, bytes));
#else
            return File.WriteAllBytesAsync(fileName, bytes, cancellationToken);
#endif
        }
    }
}
