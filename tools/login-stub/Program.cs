using System.Net;
using System.Net.Sockets;

// Minimal Fineage login stub: accepts TCP on 127.0.0.1:2106.
// Enough to stop connection refused; Init/auth packets come later.
const int Port = 2106;

var listener = new TcpListener(IPAddress.Loopback, Port);
listener.Start();
Console.WriteLine($"Fineage login stub listening on 127.0.0.1:{Port}");
Console.WriteLine("Login UI works without this. Press Login in the client to hit this socket.");
Console.WriteLine("Ctrl+C to stop.");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    _ = Task.Run(() => HandleClient(client));
}

static async Task HandleClient(TcpClient client)
{
    var ep = client.Client.RemoteEndPoint;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] connect {ep}");
    try
    {
        using (client)
        await using (var stream = client.GetStream())
        {
            var buf = new byte[4096];
            while (client.Connected)
            {
                var n = await stream.ReadAsync(buf);
                if (n == 0)
                    break;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] rx {n} bytes from {ep}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {ep} closed: {ex.Message}");
    }
}
