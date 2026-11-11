using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public sealed partial class NetOutboundSendPump : IDisposable
{
    void FailRemaining(Exception ex)
    {
        while (_queue.TryDequeue(out PendingSend pending))
            pending.Completion.TrySetException(ex);
    }

    void StopLoop_NoLock()
    {
        try { _cts?.Cancel(); } catch { /* ignore */ }
        _cts?.Dispose();
        _cts = null;
        _loop = null;
        _stream = null;
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        lock (_gate)
            StopLoop_NoLock();
        _signal.Dispose();
    }
}
