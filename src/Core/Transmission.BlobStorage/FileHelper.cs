using System.IO;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.BlobStorage
{
    internal static class FileHelper
    {
        public static Task<byte[]> ReadAllBytesAsync(string fileName)
        {
#if NETSTANDARD2_0
            return Task.Run(() => File.ReadAllBytes(fileName));
#else
            return File.ReadAllBytesAsync(fileName);
#endif
        }

        public static Task WriteAllBytesAsync(string fileName, byte[] bytes)
        {
#if NETSTANDARD2_0
            return Task.Run(() => File.WriteAllBytes(fileName, bytes));
#else
            return File.WriteAllBytesAsync(fileName, bytes);
#endif
        }
    }
}