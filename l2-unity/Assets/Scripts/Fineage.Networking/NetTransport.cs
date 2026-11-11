using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetTransport : IDisposable
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





}
