using System;
using System.Threading;
using System.Threading.Tasks;

namespace Thor.Core.Transmission.Abstractions
{
    internal class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        internal async ValueTask<Releaser> LockAsync(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);

            return new Releaser(this);
        }

        internal struct Releaser : IDisposable
        {
            private readonly AsyncLock _asyncLock;
            private bool _isDisposed;

            internal Releaser(AsyncLock asyncLock)
            {
                _asyncLock = asyncLock;
                _isDisposed = false;
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                _asyncLock?._semaphore.Release();
                _isDisposed = true;
            }
        }
    }
}
