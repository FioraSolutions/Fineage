using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public sealed partial class NetOutboundSendPump : IDisposable
{
    public void Attach(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        lock (_gate)
        {
            StopLoop_NoLock();
            _stream = stream;
            _cts = new CancellationTokenSource();
            _loop = Task.Run(() => DrainLoopAsync(_cts.Token));
        }
    }

    public Task EnqueueAsync(byte[] frame, CancellationToken ct = default)
    {
        if (frame == null)
            throw new ArgumentNullException(nameof(frame));
        if (_disposed)
            return Task.FromException(new ObjectDisposedException(nameof(NetOutboundSendPump)));

        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _queue.Enqueue(new PendingSend
        {
            Frame = frame,
            Completion = tcs,
            CallerCt = ct
        });
        _signal.Release();
        return tcs.Task;
    }

    async Task DrainLoopAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                await _signal.WaitAsync(ct).ConfigureAwait(false);

                while (_queue.TryDequeue(out PendingSend pending))
                {
                    if (pending.CallerCt.IsCancellationRequested)
                    {
                        pending.Completion.TrySetCanceled(pending.CallerCt);
                        continue;
                    }

                    Stream stream;
                    lock (_gate)
                        stream = _stream;

                    if (stream == null)
                    {
                        pending.Completion.TrySetException(new InvalidOperationException("not connected"));
                        continue;
                    }

                    try
                    {
                        await stream.WriteAsync(pending.Frame, 0, pending.Frame.Length, pending.CallerCt)
                            .ConfigureAwait(false);
                        await stream.FlushAsync(pending.CallerCt).ConfigureAwait(false);
                        pending.Completion.TrySetResult(true);
                    }
                    catch (OperationCanceledException oce)
                    {
                        pending.Completion.TrySetCanceled(oce.CancellationToken);
                    }
                    catch (Exception ex)
                    {
                        pending.Completion.TrySetException(ex);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // detach / dispose
        }
        finally
        {
            FailRemaining(new ObjectDisposedException(nameof(NetOutboundSendPump)));
        }
    }
}
