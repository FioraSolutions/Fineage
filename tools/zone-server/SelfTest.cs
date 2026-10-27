using System.Net.Sockets;
using Fineage.ZoneServer.Combat;
using Fineage.ZoneServer.Session;
using Fineage.ZoneServer.World;
using Fineage.ZoneServer.Wire;
using MessagePack;

static class SelfTest
{
    public static async Task RunAsync()
    {
        const int port = 9777;
        var world = new ZoneWorld();
        world.SeedTalkingIslandNpcs();
        TcpListener? local = null;
        try
        {
            local = new TcpListener(System.Net.IPAddress.Loopback, port);
            local.Start();
            _ = Task.Run(async () =>
            {
                var c = await local.AcceptTcpClientAsync().ConfigureAwait(false);
                await new PlayerSession(c, 1, world).RunAsync(CancellationToken.None).ConfigureAwait(false);
            });
            Console.WriteLine("[selftest] started ephemeral ZoneServer");
        }
        catch (SocketException)
        {
            Console.WriteLine("[selftest] using existing ZoneServer on :9777");
            local = null;
        }

        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(System.Net.IPAddress.Loopback, port).ConfigureAwait(false);
            var stream = client.GetStream();

            await SendAsync(stream, MsgType.Hello, new HelloMsg { ProtocolVersion = 1, ClientBuild = "selftest" })
                .ConfigureAwait(false);
            await ExpectAsync(stream, MsgType.HelloAck).ConfigureAwait(false);

            await SendAsync(stream, MsgType.Auth, new AuthMsg { Account = "dev", Password = "x" })
                .ConfigureAwait(false);
            await ExpectAsync(stream, MsgType.AuthOk).ConfigureAwait(false);

            await SendAsync(stream, MsgType.EnterWorld, new EnterWorldMsg()).ConfigureAwait(false);
            var spawn = await ExpectAsync(stream, MsgType.PlayerSpawned).ConfigureAwait(false);
            var spawned = MessagePackSerializer.Deserialize<PlayerSpawnedMsg>(spawn);
            Console.WriteLine($"[selftest] PlayerSpawned id={spawned.ObjectId} @ {spawned.X},{spawned.Y},{spawned.Z}");

            for (int i = 0; i < 2; i++)
                await ExpectAsync(stream, MsgType.NpcInfo).ConfigureAwait(false);

            await SendAsync(stream, MsgType.MoveTo, new MoveToMsg
            {
                ObjectId = spawned.ObjectId,
                DestX = spawned.X + 100,
                DestY = spawned.Y + 50,
                DestZ = spawned.Z,
                OriginX = spawned.X,
                OriginY = spawned.Y,
                OriginZ = spawned.Z
            }).ConfigureAwait(false);
            await ExpectAsync(stream, MsgType.MoveToAck).ConfigureAwait(false);

            await SendAsync(stream, MsgType.SelectTarget, new SelectTargetMsg
            {
                TargetObjectId = ZoneWorld.SwordMobObjectId
            }).ConfigureAwait(false);
            await ExpectAsync(stream, MsgType.MyTargetSelected).ConfigureAwait(false);

            await SendAsync(stream, MsgType.AttackRequest, new AttackRequestMsg()).ConfigureAwait(false);
            var attackBody = await ExpectAsync(stream, MsgType.Attack).ConfigureAwait(false);
            var attack = MessagePackSerializer.Deserialize<AttackMsg>(attackBody);
            if (attack.TargetObjectId != ZoneWorld.SwordMobObjectId)
                throw new Exception($"attack target {attack.TargetObjectId}");
            var statusBody = await ExpectAsync(stream, MsgType.StatusUpdate).ConfigureAwait(false);
            var status = MessagePackSerializer.Deserialize<StatusUpdateMsg>(statusBody);
            if (status.ObjectId != ZoneWorld.SwordMobObjectId ||
                status.Values[0] != ZoneWorld.SwordMobMaxHp - ZoneCombat.SampleMeleeDamage)
                throw new Exception($"HP sync cur={status.Values[0]}");

            Console.WriteLine("[selftest] OK Hello→Auth→EnterWorld→Move→Target→Melee→HP");
        }
        finally
        {
            local?.Stop();
        }
    }

    static async Task SendAsync<T>(NetworkStream stream, MsgType type, T msg)
    {
        byte[] body = MessagePackSerializer.Serialize(msg);
        await FrameIO.SendAsync(stream, (ushort)type, body).ConfigureAwait(false);
    }

    static async Task<byte[]> ExpectAsync(NetworkStream stream, MsgType expected)
    {
        var frame = await FrameIO.ReadAsync(stream).ConfigureAwait(false);
        if (frame == null)
            throw new Exception($"expected {expected}, got EOF");
        if (frame.Value.Type != (ushort)expected)
            throw new Exception($"expected {expected}, got {(MsgType)frame.Value.Type}");
        Console.WriteLine($"[selftest] got {expected}");
        return frame.Value.Body;
    }
}
