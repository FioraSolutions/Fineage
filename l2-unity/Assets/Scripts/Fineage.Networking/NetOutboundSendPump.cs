using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Serializes outbound Fineage Net frames onto one TCP stream (B4 outbound pump).
/// Concurrent <see cref="EnqueueAsync"/> callers queue work; a single drain loop writes
/// frames in order so partial envelopes cannot interleave.
/// </summary>
public sealed class NetOutboundSendPump : IDisposable
{
    readonly ConcurrentQueue<PendingSend> _queue = new ConcurrentQueue<PendingSend>();
    readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
    readonly object _gate = new object();
    CancellationTokenSource _cts;
    Task _loop;
    Stream _stream;
    bool _disposed;

    struct PendingSend
    {
        public byte[] Frame;
        public TaskCompletionSource<bool> Completion;
        public CancellationToken CallerCt;
    }

    /// <summary>Start (or restart) the writer loop against <paramref name="stream"/>.</summary>
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
