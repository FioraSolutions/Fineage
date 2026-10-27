using Fineage.ZoneServer.Combat;
using Fineage.ZoneServer.Session;
using Fineage.ZoneServer.World;
using Xunit;

namespace Fineage.ZoneServer.Tests;

public class ZoneWorldCombatTests
{
    [Fact]
    public void SwordMob_InAoiAtSpawn()
    {
        var world = new ZoneWorld();
        world.SeedTalkingIslandNpcs();
        int playerId = world.EnterPlayer(null!, "hero", PlayerSession.SpawnX, PlayerSession.SpawnY, PlayerSession.SpawnZ);
        var visible = world.Aoi.GetVisible(playerId);
        Assert.Contains(ZoneWorld.SwordMobObjectId, visible);
        Assert.True(world.TryGetEntity(ZoneWorld.SwordMobObjectId, out var mob));
        Assert.Equal(ZoneWorld.SwordMobMaxHp, mob!.CurHp);
        world.LeavePlayer(playerId);
    }

    [Fact]
    public void InMeleeRange_SwordMobOffsetFromSpawn()
    {
        int px = PlayerSession.SpawnX;
        int py = PlayerSession.SpawnY;
        Assert.True(ZoneCombat.InMeleeRange(px, py, px + 280, py + 280));
    }
}
