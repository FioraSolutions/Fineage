using System;
using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// Lobby select-screen mapping and playtest login host. Salvaged from the retired
/// <c>client-zone-v2.Tests</c> MessagePack codec host; only the assertions that touch
/// surviving domain code remain.
/// </summary>
public class LobbyCharacterTests
{
    [Fact]
    public void Account_cap_is_seven_including_a_pending_delete()
    {
        Assert.True(LobbyCharacterText.CanCreate(6));
        Assert.False(LobbyCharacterText.CanCreate(7));
        Assert.Equal(
            "You cannot create another character. Delete one first.",
            LobbyCharacterText.ErrorMessage("character_limit", "server text"));
        Assert.Equal("Incorrect name.", LobbyCharacterText.ErrorMessage("invalid_name", null));
        Assert.Equal("Maximum 16 letters or digits.", LobbyCharacterText.ErrorMessage("name_too_long", null));
        Assert.Equal("This name already exists.", LobbyCharacterText.ErrorMessage("name_taken", null));
        Assert.Equal("That character is pending deletion.", LobbyCharacterText.ErrorMessage("character_pending_delete", null));
    }

    [Fact]
    public void Create_validation_matches_human_fighter_and_mystic_rules()
    {
        Assert.Null(LobbyCharacterCreate.Validate("Aria", 0, 0, 0, 4, 3, 2));
        Assert.Null(LobbyCharacterCreate.Validate("Mystic1", 10, 0, 1, 6, 0, 0));
        Assert.Equal(5, LobbyCharacterCreate.HairLabels(LobbyCharacterCreate.SexMale).Length);
        Assert.Equal(7, LobbyCharacterCreate.HairLabels(LobbyCharacterCreate.SexFemale).Length);
        Assert.Equal(4, LobbyCharacterCreate.MaxHairStyle(0));
        Assert.Equal(6, LobbyCharacterCreate.MaxHairStyle(1));
        Assert.Equal(0, LobbyCharacterCreate.ClampAppearance(-1, 4));
        Assert.Equal(4, LobbyCharacterCreate.ClampAppearance(9, 4));

        Assert.Equal("name_too_long", LobbyCharacterCreate.Validate("", 0, 0, 0, 0, 0, 0));
        Assert.Equal("name_too_long", LobbyCharacterCreate.Validate("   ", 0, 0, 0, 0, 0, 0));
        Assert.Equal("name_too_long", LobbyCharacterCreate.Validate("ABCDEFGHIJKLMNOPQ", 0, 0, 0, 0, 0, 0));
        Assert.Equal("invalid_name", LobbyCharacterCreate.Validate("Bad Name", 0, 0, 0, 0, 0, 0));
        Assert.Equal("invalid_name", LobbyCharacterCreate.Validate("xxAnnouxx", 0, 0, 0, 0, 0, 0));
        Assert.Equal("creation_failed", LobbyCharacterCreate.Validate("Aria", 0, 0, 0, 5, 0, 0));
        Assert.Equal("creation_failed", LobbyCharacterCreate.Validate("Aria", 0, 0, 1, 7, 0, 0));
        Assert.Equal("creation_failed", LobbyCharacterCreate.Validate("Aria", 0, 0, 0, 0, 4, 0));
        Assert.Equal("creation_failed", LobbyCharacterCreate.Validate("Aria", 0, 0, 0, 0, 0, 3));
        Assert.Equal("class_not_supported", LobbyCharacterCreate.Validate("Aria", 1, 0, 0, 0, 0, 0));
        Assert.Equal("race_not_supported", LobbyCharacterCreate.Validate("Aria", 0, 1, 0, 0, 0, 0));
        Assert.Null(LobbyCharacterCreate.Validate("ElfF", LobbyRaceCatalog.ElfFighter, 1, 0, 0, 0, 0));
        Assert.Null(LobbyCharacterCreate.Validate("ElfM", LobbyRaceCatalog.ElfMystic, 1, 1, 0, 0, 0));
        Assert.Null(LobbyCharacterCreate.Validate("DarkF", LobbyRaceCatalog.DarkElfFighter, 2, 0, 0, 0, 0));
        Assert.Null(LobbyCharacterCreate.Validate("DarkM", LobbyRaceCatalog.DarkElfMystic, 2, 1, 0, 0, 0));
        Assert.Null(LobbyCharacterCreate.Validate("OrcF", LobbyRaceCatalog.OrcFighter, 3, 0, 0, 0, 0));
        Assert.Null(LobbyCharacterCreate.Validate("OrcM", LobbyRaceCatalog.OrcMystic, 3, 1, 6, 0, 0));
        Assert.Null(LobbyCharacterCreate.Validate("Dwarf", LobbyRaceCatalog.DwarfFighter, 4, 0, 0, 0, 0));
        Assert.Equal("race_not_supported", LobbyCharacterCreate.Validate("Aria", LobbyRaceCatalog.OrcFighter, 0, 0, 0, 0, 0));
        Assert.Equal("race_not_supported", LobbyCharacterCreate.Validate("Aria", LobbyRaceCatalog.HumanMystic, 4, 0, 0, 0, 0));
        Assert.Equal("class_not_supported", LobbyCharacterCreate.Validate("Aria", 99, 4, 0, 0, 0, 0));
        Assert.Equal(44, LobbyRaceCatalog.ClassIdFor("Orc", "Fighter"));
        Assert.Equal(49, LobbyRaceCatalog.ClassIdFor("Orc", "Mystic"));
        Assert.Equal(53, LobbyRaceCatalog.ClassIdFor("Dwarf", "Fighter"));
        Assert.Equal(-1, LobbyRaceCatalog.ClassIdFor("Dwarf", "Mystic"));
        Assert.Equal(3, LobbyRaceCatalog.RaceId("Orc"));
        Assert.Equal("Orc Fighter", LobbyCharacterText.ClassLabel(LobbyRaceCatalog.OrcFighter));
        Assert.Single(LobbyRaceCatalog.ClassNames("Dwarf"));
        Assert.Equal(2, LobbyRaceCatalog.ClassNames("Orc").Length);
        Assert.Equal("sex_not_supported", LobbyCharacterCreate.Validate("Aria", 0, 0, 2, 0, 0, 0));
        Assert.Equal("This name already exists.", LobbyCharacterText.ErrorMessage("name_taken", null));
        Assert.Equal("Character creation failed.", LobbyCharacterText.ErrorMessage("creation_failed", null));
        Assert.Equal("Characters cannot be created on this server.", LobbyCharacterText.ErrorMessage("create_not_allowed", null));
    }

    [Fact]
    public void Empty_roster_maps_to_create_ui_status_not_login_overlay()
    {
        LobbyCharacter[] empty = LobbyCharacterMap.FromSummaries(Array.Empty<NetCharacterSummary>());
        Assert.Empty(empty);
        Assert.Equal(-1, LobbyCharacterMap.PreferredIndex(empty, 0));
        Assert.True(LobbyCharacterText.CanCreate(0));
        Assert.Equal("No characters yet.", LobbyCharacterText.EmptyRosterStatus);
        Assert.NotEqual("No characters on this account.", LobbyCharacterText.EmptyRosterStatus);
    }

    [Fact]
    public void Public_login_host_is_evpoint_not_loopback()
    {
        Assert.True(FineageWireConfig.IsPublicLoginHost(PlaytestEndpoint.DefaultLoginHost));
        Assert.Equal("login.fineage.evpoint.by", PlaytestEndpoint.DefaultLoginHost);
        Assert.Equal(2106, PlaytestEndpoint.DefaultLoginPort);
        Assert.False(FineageWireConfig.IsPublicLoginHost("127.0.0.1"));
        Assert.False(FineageWireConfig.IsPublicLoginHost("localhost"));
    }
}
