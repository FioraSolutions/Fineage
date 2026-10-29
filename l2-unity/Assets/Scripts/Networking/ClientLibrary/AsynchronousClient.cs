using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>Legacy Interlude game-socket client (used only by <see cref="GameClient"/>; login is Fineage-Backend).</summary>
public class AsynchronousClient
{
    private System.Threading.CancellationTokenSource _cts;

    private int _state;
    private string _ip;
    private int _port;

    private Socket _socket;
    public NetworkStream _stream;
    private DefaultClient _client;

    private ClientPacketHandler _clientPacketHandler;
    private ServerPacketHandler _serverPacketHandler;
    private GameClientReceiving _gameReceiving;

    private bool _cryptEnabled = false;
    private bool _initPacket = true;
    private bool _initPacketEnabled;

    public bool InitPacket { get => _initPacket; set => _initPacket = value; }
    public bool IsConnected => Volatile.Read(ref _state) == 2;

    public bool CryptEnabled
    {
        get => _cryptEnabled;
        set
        {
            // Debug.Log("Crypt" + (value ? " enabled." : " disabled."));
            _cryptEnabled = value;
        }
    }

    private NetworkRuntime _network;

    public AsynchronousClient(string ip, int port, DefaultClient client, ClientPacketHandler clientPacketHandler, ServerPacketHandler serverPacketHandler, bool enableInitPacket, NetworkRuntime network)
    {
        _ip = ip;
        _port = port;
        _client = client;

        _clientPacketHandler = clientPacketHandler;
        _serverPacketHandler = serverPacketHandler;

        _clientPacketHandler.SetClient(this);
        _serverPacketHandler.SetClient(this, _clientPacketHandler);

        _initPacketEnabled = enableInitPacket;
        _initPacket = enableInitPacket;
        _network = network;

        _network.SendGame.SetPacketHandler(_clientPacketHandler);
        _network.IncomingGame.SetPacketHandler(_serverPacketHandler);
        _gameReceiving = new GameClientReceiving(this, _network.IncomingGame);
    }

    public async Task<bool> Connect()
    {
        if (Interlocked.CompareExchange(ref _state, 1, 0) != 0)
            return false;

        _cts = new CancellationTokenSource();

        try
        {
            string ipStr = !string.IsNullOrWhiteSpace(_client.ServerIp) ? _client.ServerIp : _ip;
            int port = _client.ServerPort != 0 ? _client.ServerPort : _port;
            IPAddress ip = EndpointAddress.Resolve(ipStr);
            _socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            if (_initPacketEnabled)
                _initPacket = true;

            await _socket.ConnectAsync(ip, port);

            _stream = new NetworkStream(_socket, ownsSocket: false);

            Volatile.Write(ref _state, 2);

            _gameReceiving.StartReceiving(_socket, _cts.Token);

            return true;
        }
        catch (Exception ex)
        {
            string host = _client != null && !string.IsNullOrWhiteSpace(_client.ServerIp) ? _client.ServerIp : _ip;
            Debug.LogWarning("Game connection failed for " + host + ": " + ex.Message);
            Disconnect();
            EventProcessor events = _network != null ? _network.Events : NetworkingPacketServices.Events;
            events.QueueEvent(() => _client.OnConnectionFailed());
            return false;
        }
    }

    public void Disconnect()
    {
        int prev = Interlocked.Exchange(ref _state, 3);
        if (prev == 0)
        {
            Interlocked.Exchange(ref _state, 0);
            return;
        }

        CloseSocket();

        DisposeQueue();

        EventProcessor events = _network != null ? _network.Events : NetworkingPacketServices.Events;
        DefaultClient owner = _client;
        events.QueueEvent(() => owner.OnDisconnectFrom(this));

        Interlocked.Exchange(ref _state, 0);
    }

    private void DisposeQueue()
    {
        _network.SendGame.Stop();
        _network.IncomingGame.Stop();
    }

    public void SendPacket(IOutgoingPacket packet)
    {
        TrySendPacket(packet);
    }

    /// <summary>Write one framed packet. Returns false if socket dead or write threw.</summary>
    public bool TrySendPacket(IOutgoingPacket packet)
    {
        try
        {
            if (packet == null)
                return false;
            if (_socket == null || !_socket.Connected || _stream == null)
            {
                LobbyFlowLog.Warn("TrySendPacket skip — socket dead state=" + Volatile.Read(ref _state) +
                    " connectedFlag=" + (_socket != null && _socket.Connected));
                return false;
            }

            byte[] data = packet.GetData();
            if (data == null)
                return false;
            int totalSize = data.Length + 2;
            if (totalSize > ushort.MaxValue)
                throw new InvalidOperationException("Packet too large");

            ushort sizeHeader = (ushort)totalSize;
            byte[] header = BitConverter.GetBytes(sizeHeader);

            _stream.Write(header, 0, 2);
            _stream.Write(data, 0, data.Length);
            _stream.Flush();
            return true;
        }
        catch (Exception e)
        {
            LobbyFlowLog.Exception("TrySendPacket", e);
            return false;
        }
    }

    private void CloseSocket()
    {
        try { _serverPacketHandler.CancelTokens(); } catch { }
        try { _cts?.Cancel(); } catch { }
        try { _socket?.Shutdown(SocketShutdown.Both); } catch { }
        try { _stream?.Dispose(); } catch { }
        try { _socket?.Close(); } catch { }
        try { _socket?.Dispose(); } catch { }
        _stream = null;
        _socket = null;
    }
}
