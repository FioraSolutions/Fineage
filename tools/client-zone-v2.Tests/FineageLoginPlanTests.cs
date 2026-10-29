using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

public class FineageLoginPlanTests
{
    [Fact]
    public void Empty_roster_is_character_select_not_an_error()
    {
        ZoneV2CharacterSummary selected;
        string error;
        FineageLoginPlan plan = FineageLoginSession.Plan(null, "stale-name", false, out selected, out error);

        Assert.Equal(FineageLoginPlan.CharacterSelect, plan);
        Assert.Null(error);

        plan = FineageLoginSession.Plan(Array.Empty<ZoneV2CharacterSummary>(), null, true, out selected, out error);
        Assert.Equal(FineageLoginPlan.CharacterSelect, plan);
        Assert.Null(error);
        Assert.Equal("No characters yet.", LobbyCharacterText.EmptyRosterStatus);
        Assert.Equal(-1, LobbyCharacterMap.PreferredIndex(Array.Empty<LobbyCharacter>(), 0));
        Assert.True(LobbyCharacterText.CanCreate(0));
    }

    [Fact]
    public void Public_player_with_characters_stays_on_character_select()
    {
        ZoneV2CharacterSummary[] rows =
        {
            new ZoneV2CharacterSummary { Name = "Mage", CharacterId = 4 }
        };

        ZoneV2CharacterSummary selected;
        string error;
        FineageLoginPlan plan = FineageLoginSession.Plan(rows, null, true, out selected, out error);

        Assert.Equal(FineageLoginPlan.CharacterSelect, plan);
        Assert.Null(error);
    }

    [Fact]
    public void Headless_single_character_still_hands_off()
    {
        ZoneV2CharacterSummary[] rows =
        {
            new ZoneV2CharacterSummary { Name = "Mage", CharacterId = 4 }
        };

        ZoneV2CharacterSummary selected;
        string error;
        FineageLoginPlan plan = FineageLoginSession.Plan(rows, null, false, out selected, out error);

        Assert.Equal(FineageLoginPlan.Handoff, plan);
        Assert.Null(error);
        Assert.Equal("Mage", selected.Name);
        Assert.Equal(4, selected.CharacterId);
    }

    [Fact]
    public void Missing_character_fails_without_empty_roster_text()
    {
        ZoneV2CharacterSummary[] rows =
        {
            new ZoneV2CharacterSummary { Name = "A" },
            new ZoneV2CharacterSummary { Name = "B" }
        };

        ZoneV2CharacterSummary selected;
        string error;
        FineageLoginPlan plan = FineageLoginSession.Plan(rows, "Nope", false, out selected, out error);

        Assert.Equal(FineageLoginPlan.Failed, plan);
        Assert.NotNull(error);
        Assert.DoesNotContain("No characters on this account", error);
    }

    [Fact]
    public void Public_login_host_is_not_loopback()
    {
        Assert.True(FineageLoginSession.IsPublicLoginHost("login.fineage.evpoint.by"));
        Assert.True(FineageLoginSession.IsPublicLoginHost(PlaytestEndpoint.DefaultLoginHost));
        Assert.False(FineageLoginSession.IsPublicLoginHost("127.0.0.1"));
        Assert.False(FineageLoginSession.IsPublicLoginHost("localhost"));
        Assert.False(FineageLoginSession.IsPublicLoginHost("::1"));
        Assert.False(FineageLoginSession.IsPublicLoginHost(""));
    }
}
