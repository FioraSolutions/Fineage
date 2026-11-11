using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetTransport : IDisposable
{
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
}
