using Fineage.ZoneServer.Session;
using Fineage.ZoneServer.World;
using Fineage.ZoneServer.Wire;
using Xunit;

namespace Fineage.ZoneServer.Tests;

public class ZoneWorldPresenceTests
{
    [Fact]
    public void SeedTalkingIslandNpcs_NearSpawn_InAoi()
    {
        var world = new ZoneWorld();
        world.SeedTalkingIslandNpcs();

        int probeId = world.EnterPlayer(null!, "probe", PlayerSession.SpawnX, PlayerSession.SpawnY, PlayerSession.SpawnZ);
        var visible = world.Aoi.GetVisible(probeId);
        Assert.Contains(20_001, visible);
        Assert.DoesNotContain(20_002, visible);
        world.LeavePlayer(probeId);
    }

    [Fact]
    public void BuildPresenceMsg_Npc_UsesNpcInfo()
    {
        var entity = new ZoneEntity
        {
            ObjectId = 20_001,
            Kind = ZoneEntityKind.Npc,
            Name = "Guard",
            X = 1,
            Y = 2,
            Z = 3,
            NpcTemplateId = 30_001
        };
        var (type, msg) = ZoneWorld.BuildPresenceMsg(entity);
        Assert.Equal(MsgType.NpcInfo, type);
        Assert.IsType<NpcInfoMsg>(msg);
    }
}
