using Xunit;

namespace Fineage.Client.Domain.Tests;

public class WorldEntityRegistryTests
{
    [Fact]
    public void RegisterPlayer_is_findable_in_all_and_players()
    {
        var reg = new WorldEntityRegistry<string>();
        reg.RegisterPlayer(1, "p");
        Assert.True(reg.Contains(1));
        Assert.True(reg.ContainsPlayer(1));
        Assert.False(reg.ContainsNpc(1));
        Assert.Equal("p", reg.GetOrNull(1));
    }

    [Fact]
    public void TryRegisterUser_skips_when_already_present()
    {
        var reg = new WorldEntityRegistry<string>();
        reg.RegisterPlayer(2, "first");
        Assert.False(reg.TryRegisterUser(2, "second"));
        Assert.Equal("first", reg.GetOrNull(2));
        Assert.True(reg.TryRegisterUser(3, "u"));
        Assert.Equal("u", reg.GetOrNull(3));
    }

    [Fact]
    public void RegisterNpc_and_item_buckets()
    {
        var reg = new WorldEntityRegistry<string>();
        reg.RegisterNpc(10, "npc");
        reg.RegisterItem(11, "item");
        Assert.True(reg.ContainsNpc(10));
        Assert.False(reg.ContainsNpc(11));
        Assert.Equal("npc", reg.GetOrNull(10));
        Assert.Equal("item", reg.GetOrNull(11));
    }

    [Fact]
    public void Unregister_and_Clear_remove_entries()
    {
        var reg = new WorldEntityRegistry<string>();
        reg.RegisterPlayer(1, "p");
        reg.RegisterNpc(2, "n");
        reg.Unregister(1);
        Assert.Null(reg.GetOrNull(1));
        Assert.Equal("n", reg.GetOrNull(2));
        reg.Clear();
        Assert.Null(reg.GetOrNull(2));
    }

    [Fact]
    public void ForEach_variants_visit_buckets()
    {
        var reg = new WorldEntityRegistry<string>();
        reg.RegisterPlayer(1, "p");
        reg.RegisterNpc(2, "n");
        reg.RegisterItem(3, "i");

        int all = 0, players = 0, npcs = 0;
        reg.ForEach(_ => all++);
        reg.ForEachPlayer(_ => players++);
        reg.ForEachNpc(_ => npcs++);
        Assert.Equal(3, all);
        Assert.Equal(1, players);
        Assert.Equal(1, npcs);
    }
}
