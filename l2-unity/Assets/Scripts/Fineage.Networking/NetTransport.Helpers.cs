using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetTransport : IDisposable
{
    public Task SendAsync(ushort opcode, byte[] payload, uint sequence = 0, CancellationToken ct = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(NetTransport));
        if (_stream == null)
            throw new InvalidOperationException("not connected");
        var envelope = new PacketEnvelope
        {
            Opcode = opcode,
            Flags = (ushort)PacketFlags.Reliable,
            RequestSequence = sequence,
            Payload = payload ?? Array.Empty<byte>()
        };
        byte[] frame = NetFraming.Encode(envelope);
        return _outbound.EnqueueAsync(frame, ct);
    }

    async Task ReadLoopAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                PacketEnvelope envelope = await NetFraming.ReadAsync(_stream, ct).ConfigureAwait(false);
                if (envelope == null)
                    break;
                _onPacket(envelope);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (IOException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogWarning("[Net] read ended: " + ex.Message);
        }
        finally
        {
            _onClosed?.Invoke("closed");
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        try { _cts?.Cancel(); } catch { /* ignore */ }
        _outbound.Dispose();
        try { _stream?.Dispose(); } catch { /* ignore */ }
        try { _client?.Dispose(); } catch { /* ignore */ }
        _stream = null;
        _client = null;
        _cts = null;
    }
}
