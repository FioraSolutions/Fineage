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
        if (CombatTrace.Enabled)
            CombatTrace.Write("tx.pkt", "op=" + NetPacketTrace.OpName(opcode) + " seq=" + sequence + " len=" + envelope.Payload.Length);
        return _outbound.EnqueueAsync(frame, ct);
    }

    async Task ReadLoopAsync(CancellationToken ct)
    {
        string reason = "remote-closed";
        try
        {
            while (!ct.IsCancellationRequested)
            {
                PacketEnvelope envelope;
                using (NetReceiveProfiling.Read.Auto())
                    envelope = await NetFramingPooled.ReadAsync(_stream, _readLengthScratch, ct).ConfigureAwait(false);
                if (envelope == null)
                {
                    reason = _disposed ? "disposed" : "eof";
                    break;
                }

                try
                {
                    _onPacket(envelope);
                }
                catch (Exception ex)
                {
                    // Frame already consumed — a decode/handler error must not drop TCP.
                    UnityEngine.Debug.LogError("[Net] packet handler opcode=" + envelope.Opcode + ": " + ex);
                }
            }

            if (ct.IsCancellationRequested)
                reason = _disposed ? "disposed" : "canceled";
        }
        catch (OperationCanceledException)
        {
            reason = _disposed ? "disposed" : "canceled";
        }
        catch (IOException ex)
        {
            reason = _disposed ? "disposed" : ("io:" + ex.Message);
        }
        catch (ObjectDisposedException)
        {
            reason = "disposed";
        }
        catch (Exception ex)
        {
            reason = "error:" + ex.Message;
            UnityEngine.Debug.LogWarning("[Net] read ended: " + ex.Message);
        }
        finally
        {
            _onClosed?.Invoke(reason);
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
