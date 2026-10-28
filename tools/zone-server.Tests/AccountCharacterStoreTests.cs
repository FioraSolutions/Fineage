using Fineage.ZoneServer.Accounts;
using Xunit;

namespace Fineage.ZoneServer.Tests;

public sealed class AccountCharacterStoreTests
{
    [Fact]
    public void Create_human_fighter_adds_to_account()
    {
        var store = new AccountCharacterStore();
        var result = store.TryCreate(
            "alice",
            "Hero",
            HumanFighterDefaults.RaceHuman,
            0,
            HumanFighterDefaults.ClassFighter,
            0,
            0,
            0,
            out var created);

        Assert.Equal(StubCreateResult.Ok, result);
        Assert.NotNull(created);
        Assert.Equal("Hero", created!.Name);
        Assert.Single(store.GetCharacters("alice"));
    }

    [Fact]
    public void Rejects_non_human_fighter()
    {
        var store = new AccountCharacterStore();
        var result = store.TryCreate("bob", "Mage1", 0, 0, 10, 0, 0, 0, out _);
        Assert.Equal(StubCreateResult.NotHumanFighter, result);
    }

    [Fact]
    public void Name_collision_fails()
    {
        var store = new AccountCharacterStore();
        Assert.Equal(StubCreateResult.Ok, store.TryCreate("a", "Dup", 0, 0, 0, 0, 0, 0, out _));
        Assert.Equal(StubCreateResult.NameAlreadyExists, store.TryCreate("b", "Dup", 0, 0, 0, 0, 0, 0, out _));
    }
}
