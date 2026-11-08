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
    readonly NetOutboundSendPump _outbound = new NetOutboundSendPump();
    bool _disposed;

    public bool Connected => _client != null && _client.Connected;

    public NetTransport(Action<PacketEnvelope> onPacket, Action<string> onClosed = null)
    {
        _onPacket = onPacket ?? throw new ArgumentNullException(nameof(onPacket));
        _onClosed = onClosed;
    }

    public async Task ConnectAsync(string host, int port, CancellationToken ct = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(NetTransport));

        try { _cts?.Cancel(); } catch { /* ignore */ }

        var client = new TcpClient();
        await client.ConnectAsync(host, port).ConfigureAwait(false);
        NetworkStream stream = client.GetStream();

        // Stop the writer against the old stream before disposing it.
        _outbound.Attach(stream);

        TcpClient oldClient = _client;
        NetworkStream oldStream = _stream;
        _client = client;
        _stream = stream;
        try { oldStream?.Dispose(); } catch { /* ignore */ }
        try { oldClient?.Dispose(); } catch { /* ignore */ }

        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _readLoop = Task.Run(() => ReadLoopAsync(_cts.Token), _cts.Token);
    }

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
