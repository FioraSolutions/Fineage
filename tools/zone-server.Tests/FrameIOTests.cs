using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using Fineage.ZoneServer.Wire;
using MessagePack;
using Xunit;

namespace Fineage.ZoneServer.Tests;

public class FrameIOTests
{
    /// <summary>Create a loopback TCP pair for FrameIO testing.</summary>
    static async Task<(NetworkStream client, NetworkStream server, TcpClient c, TcpClient s)> CreatePairAsync()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;

        var connectTask = Task.Run(async () =>
        {
            var c = new TcpClient();
            await c.ConnectAsync(IPAddress.Loopback, port);
            return c;
        });

        var server = await listener.AcceptTcpClientAsync();
        var client = await connectTask;
        listener.Stop();

        return (client.GetStream(), server.GetStream(), client, server);
    }

    [Fact]
    public async Task SendAndRead_RoundTrips_EmptyBody()
    {
        var (cs, ss, c, s) = await CreatePairAsync();
        try
        {
            await FrameIO.SendAsync(cs, 42, Array.Empty<byte>());
            var frame = await FrameIO.ReadAsync(ss);

            Assert.NotNull(frame);
            Assert.Equal(42, (int)frame!.Value.Type);
            Assert.Empty(frame.Value.Body);
        }
        finally { c.Dispose(); s.Dispose(); }
    }

    [Fact]
    public async Task SendAndRead_RoundTrips_WithBody()
    {
        var (cs, ss, c, s) = await CreatePairAsync();
        try
        {
            byte[] body = [0xDE, 0xAD, 0xBE, 0xEF, 0x01, 0x02];
            await FrameIO.SendAsync(cs, 1001, body);
            var frame = await FrameIO.ReadAsync(ss);

            Assert.NotNull(frame);
            Assert.Equal(1001, (int)frame!.Value.Type);
            Assert.Equal(body, frame.Value.Body);
        }
        finally { c.Dispose(); s.Dispose(); }
    }

    [Fact]
    public async Task SendAndRead_MessagePackPayload_RoundTrips()
    {
        var (cs, ss, c, s) = await CreatePairAsync();
        try
        {
            var original = new HelloMsg { ProtocolVersion = 7, ClientBuild = "test-build" };
            byte[] body = MessagePackSerializer.Serialize(original);

            await FrameIO.SendAsync(cs, (ushort)MsgType.Hello, body);
            var frame = await FrameIO.ReadAsync(ss);

            Assert.NotNull(frame);
            Assert.Equal((ushort)MsgType.Hello, frame!.Value.Type);

            var decoded = MessagePackSerializer.Deserialize<HelloMsg>(frame.Value.Body);
            Assert.Equal(7, decoded.ProtocolVersion);
            Assert.Equal("test-build", decoded.ClientBuild);
        }
        finally { c.Dispose(); s.Dispose(); }
    }

    [Fact]
    public async Task ReadAsync_ReturnsNull_OnDisconnect()
    {
        var (cs, ss, c, s) = await CreatePairAsync();
        try
        {
            c.Dispose();
            var frame = await FrameIO.ReadAsync(ss);
            Assert.Null(frame);
        }
        finally { s.Dispose(); }
    }

    [Fact]
    public async Task MultipleFrames_ArriveInOrder()
    {
        var (cs, ss, c, s) = await CreatePairAsync();
        try
        {
            for (ushort i = 1; i <= 5; i++)
            {
                byte[] body = [(byte)i];
                await FrameIO.SendAsync(cs, i, body);
            }

            for (ushort i = 1; i <= 5; i++)
            {
                var frame = await FrameIO.ReadAsync(ss);
                Assert.NotNull(frame);
                Assert.Equal(i, frame!.Value.Type);
                Assert.Equal([(byte)i], frame.Value.Body);
            }
        }
        finally { c.Dispose(); s.Dispose(); }
    }

    [Fact]
    public void HeaderSize_IsFour()
    {
        Assert.Equal(4, FrameIO.HeaderSize);
    }
}
