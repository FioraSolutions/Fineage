using System.Net;
using System.Net.Sockets;
using Fineage.LoginStub;
using Xunit;

namespace Fineage.LoginStub.Tests;

public class FramedTests
{
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
    public async Task SendAndRead_RoundTrips()
    {
        var (cs, ss, c, s) = await CreatePairAsync();
        try
        {
            byte[] body = [0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07];
            await Framed.SendAsync(cs, body);
            var received = await Framed.ReadBodyAsync(ss);

            Assert.NotNull(received);
            Assert.Equal(body, received!);
        }
        finally { c.Dispose(); s.Dispose(); }
    }

    [Fact]
    public async Task ReadBodyAsync_ReturnsNull_OnDisconnect()
    {
        var (cs, ss, c, s) = await CreatePairAsync();
        try
        {
            c.Dispose();
            var result = await Framed.ReadBodyAsync(ss);
            Assert.Null(result);
        }
        finally { s.Dispose(); }
    }

    [Fact]
    public async Task SendAsync_FrameHeader_IsLittleEndian_TotalLength()
    {
        var (cs, ss, c, s) = await CreatePairAsync();
        try
        {
            byte[] body = [0xAA, 0xBB];
            await Framed.SendAsync(cs, body);

            byte[] raw = new byte[4];
            int read = 0;
            while (read < 4)
                read += await ss.ReadAsync(raw.AsMemory(read, 4 - read));

            int total = raw[0] | (raw[1] << 8);
            Assert.Equal(4, total); // 2 header + 2 body
            Assert.Equal(0xAA, raw[2]);
            Assert.Equal(0xBB, raw[3]);
        }
        finally { c.Dispose(); s.Dispose(); }
    }
}
