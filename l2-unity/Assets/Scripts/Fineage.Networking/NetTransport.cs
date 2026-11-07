using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Fineage.Net.Contract;

/// <summary>TCP PacketEnvelope transport for Fineage Net (LiteNetLib later).</summary>
public sealed class NetTransport : IDisposable
{
    TcpClient _client;
    NetworkStream _stream;
    CancellationTokenSource _cts;
    Task _readLoop;
    readonly Action<PacketEnvelope> _onPacket;
    readonly Action<string> _onClosed;

    public bool Connected => _client != null && _client.Connected;

    public NetTransport(Action<PacketEnvelope> onPacket, Action<string> onClosed = null)
    {
        _onPacket = onPacket ?? throw new ArgumentNullException(nameof(onPacket));
        _onClosed = onClosed;
    }

    public async Task ConnectAsync(string host, int port, CancellationToken ct = default)
    {
        Dispose();
        _client = new TcpClient();
        await _client.ConnectAsync(host, port).ConfigureAwait(false);
        _stream = _client.GetStream();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _readLoop = Task.Run(() => ReadLoopAsync(_cts.Token), _cts.Token);
    }

    public async Task SendAsync(ushort opcode, byte[] payload, uint sequence = 0, CancellationToken ct = default)
    {
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
        await _stream.WriteAsync(frame, 0, frame.Length, ct).ConfigureAwait(false);
        await _stream.FlushAsync(ct).ConfigureAwait(false);
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
        try { _cts?.Cancel(); } catch { /* ignore */ }
        try { _stream?.Dispose(); } catch { /* ignore */ }
        try { _client?.Dispose(); } catch { /* ignore */ }
        _stream = null;
        _client = null;
        _cts = null;
    }
}
