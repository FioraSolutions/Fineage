using Xunit;

namespace Fineage.Client.Domain.Tests;

public class LobbyRaceCatalogTests
{
    [Fact]
    public void ClassNames_dwarf_is_fighter_only()
    {
        string[] names = LobbyRaceCatalog.ClassNames("Dwarf");
        Assert.Equal(LobbyRaceCatalog.FighterOnly, names);
        Assert.DoesNotContain("Mystic", names);
    }

    [Fact]
    public void ClassNames_human_includes_mystic()
    {
        string[] names = LobbyRaceCatalog.ClassNames("Human");
        Assert.Equal(LobbyRaceCatalog.FighterAndMystic, names);
        Assert.Contains("Mystic", names);
    }

    [Fact]
    public void ClassIdFor_dwarf_mystic_returns_negative()
    {
        Assert.Equal(-1, LobbyRaceCatalog.ClassIdFor("Dwarf", "Mystic"));
        Assert.Equal(LobbyRaceCatalog.DwarfFighter, LobbyRaceCatalog.ClassIdFor("Dwarf", "Fighter"));
    }
}
