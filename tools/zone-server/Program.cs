using System.Net;
using System.Net.Sockets;
using Fineage.ZoneServer.Session;

// Fineage ZoneServer skeleton (wire v2 + MessagePack).
// Ports differ from Interlude stub so both can run: login-stub 2106/7777, zone 9106/9777.
const int LoginPort = 9106; // reserved — Auth currently on game socket for MVP
const int GamePort = 9777;

Console.WriteLine($"Fineage ZoneServer v2  game 127.0.0.1:{GamePort}  (login reserved :{LoginPort})");
Console.WriteLine("Flow: Hello → Auth → EnterWorld → MoveTo ↔ MoveToAck");
Console.WriteLine("Client dual-adapter not wired yet — smoke with --selftest.");
Console.WriteLine("Ctrl+C to stop.");

if (args.Contains("--selftest"))
{
    await SelfTest.RunAsync();
    return;
}

var listener = new TcpListener(IPAddress.Loopback, GamePort);
listener.Start();
int nextSession = 1;

_ = Task.Run(async () =>
{
    while (true)
    {
        var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
        int sid = Interlocked.Increment(ref nextSession);
        var session = new PlayerSession(client, sid);
        _ = Task.Run(() => session.RunAsync(CancellationToken.None));
    }
});

await Task.Delay(Timeout.Infinite);
