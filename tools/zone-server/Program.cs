using System.Net;
using System.Net.Sockets;
using Fineage.ZoneServer;
using Fineage.ZoneServer.Session;
using Fineage.ZoneServer.World;

// Local MessagePack skeleton. Canonical V2 Login and Zone:
// https://github.com/FioraSolutions/Fineage-Backend — Login :9106, Zone :9777.
// Ports differ from the Interlude stub so both can run: login-stub 2106/7777, this process 9777.
const int LoginPort = 9106; // Fineage-Backend Login; this skeleton does not bind it
const int GamePort = 9777;

ZoneLog.Configure(args.Contains("--verbose") || args.Contains("-v"));

if (args.Contains("--selftest"))
{
    await SelfTest.RunAsync();
    return;
}

var world = new ZoneWorld();
world.SeedTalkingIslandNpcs();

var listener = new TcpListener(IPAddress.Loopback, GamePort);
listener.Start();
ZoneLog.Info($"listen bind game=127.0.0.1:{GamePort} (canonical login https://github.com/FioraSolutions/Fineage-Backend :{LoginPort})");
Console.WriteLine("Flow: Hello → Auth → EnterWorld → MoveTo ↔ MoveToAck");
Console.WriteLine("Client dual-adapter not wired yet — smoke with --selftest.");
Console.WriteLine(ZoneLog.Verbose
    ? "Verbose: on (Debug)."
    : "Info milestones; pass --verbose or FINEAGE_ZONE_DEBUG=1 for Debug.");
Console.WriteLine("File: logs/zone.log  |  Ctrl+C to stop.");

int nextSession = 1;

_ = Task.Run(async () =>
{
    while (true)
    {
        var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
        int sid = Interlocked.Increment(ref nextSession);
        var session = new PlayerSession(client, sid, world);
        _ = Task.Run(async () =>
        {
            try { await session.RunAsync(CancellationToken.None).ConfigureAwait(false); }
            catch (Exception ex) { ZoneLog.Error($"session={sid} handler: {ex.Message}"); }
        });
    }
});

await Task.Delay(Timeout.Infinite);
