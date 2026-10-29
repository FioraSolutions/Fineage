using System;
using MessagePack;
using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

/// <summary>
/// Lobby wire and select-screen mapping. Unity EditMode cannot reference Assembly-CSharp;
/// this project is the repo's client wire test setup and runs in build-and-test.
/// </summary>
public class ZoneV2LobbyWireTests
{
    [Fact]
    public void Lobby_opcodes_stay_off_the_zone_and_v2_lobby_ids()
    {
        Assert.Equal(20, (ushort)ZoneV2MsgType.Register);
        Assert.Equal(21, (ushort)ZoneV2MsgType.CreateCharacter);
        Assert.Equal(22, (ushort)ZoneV2MsgType.ListCharacters);
        Assert.Equal(23, (ushort)ZoneV2MsgType.SelectCharacter);
        Assert.Equal(24, (ushort)ZoneV2MsgType.DeleteCharacter);
        Assert.Equal(25, (ushort)ZoneV2MsgType.RestoreCharacter);
        Assert.Equal(1020, (ushort)ZoneV2MsgType.RegisterOk);
        Assert.Equal(1021, (ushort)ZoneV2MsgType.CharacterCreated);
        Assert.Equal(1022, (ushort)ZoneV2MsgType.CharacterList);
        Assert.Equal(1023, (ushort)ZoneV2MsgType.Handoff);
        Assert.Equal(1024, (ushort)ZoneV2MsgType.LoginOk);
        Assert.Equal(1025, (ushort)ZoneV2MsgType.CharacterDeleted);
        Assert.Equal(1026, (ushort)ZoneV2MsgType.CharacterDeleteFailed);
        Assert.Equal(1099, (ushort)ZoneV2MsgType.Error);
        Assert.Equal(1005, (ushort)ZoneV2MsgType.CharInfo);
        Assert.Equal(1006, (ushort)ZoneV2MsgType.NpcInfo);
        Assert.Equal(1007, (ushort)ZoneV2MsgType.DeleteObject);
        Assert.Equal(1014, (ushort)ZoneV2MsgType.ItemList);
        Assert.Equal(1015, (ushort)ZoneV2MsgType.InventoryUpdate);
        Assert.Equal(1016, (ushort)ZoneV2MsgType.UserInfo);
        Assert.False(Enum.IsDefined(typeof(ZoneV2MsgType), (ushort)1030));
        Assert.False(Enum.IsDefined(typeof(ZoneV2MsgType), (ushort)1031));
        Assert.False(Enum.IsDefined(typeof(ZoneV2MsgType), (ushort)1032));
        Assert.False(Enum.IsDefined(typeof(ZoneV2MsgType), (ushort)1033));
    }

    [Fact]
    public void Character_list_round_trips_appearance_position_and_delete_timer()
    {
        var list = new BackendCharacterListMsg
        {
            Characters =
            [
                new BackendCharacterSummaryMsg
                {
                    CharacterId = 4,
                    Name = "Aria",
                    ClassId = 10,
                    Race = 0,
                    Sex = 1,
                    Level = 3,
                    Slot = 1,
                    HairStyle = 6,
                    HairColor = 2,
                    Face = 1,
                    X = -84300,
                    Y = 244600,
                    Z = -3730,
                    DeleteSecondsLeft = 0,
                    IsLastPlayed = true
                },
                new BackendCharacterSummaryMsg
                {
                    CharacterId = 9,
                    Name = "Bron",
                    ClassId = 0,
                    Race = 0,
                    Sex = 0,
                    Level = 1,
                    Slot = 0,
                    HairStyle = 2,
                    HairColor = 3,
                    Face = 0,
                    X = 1,
                    Y = 2,
                    Z = 3,
                    DeleteSecondsLeft = 90061,
                    IsLastPlayed = false
                }
            ]
        };

        Assert.True(ZoneV2Messages.TryDecodeCharacterList(Pack(list), out ZoneV2CharacterSummary[] rows));
        Assert.Equal(2, rows.Length);
        Assert.Equal(4, rows[0].CharacterId);
        Assert.Equal("Aria", rows[0].Name);
        Assert.Equal(10, rows[0].ClassId);
        Assert.Equal(1, rows[0].Sex);
        Assert.Equal(6, rows[0].HairStyle);
        Assert.Equal(2, rows[0].HairColor);
        Assert.Equal(1, rows[0].Face);
        Assert.Equal(-84300, rows[0].X);
        Assert.Equal(244600, rows[0].Y);
        Assert.Equal(-3730, rows[0].Z);
        Assert.Equal(0, rows[0].DeleteSecondsLeft);
        Assert.True(rows[0].IsLastPlayed);
        Assert.Equal(90061, rows[1].DeleteSecondsLeft);
        Assert.False(rows[1].IsLastPlayed);
        Assert.Equal(0, rows[1].Slot);

        LobbyCharacter[] mapped = LobbyCharacterMap.FromSummaries(rows);
        Assert.Equal(0, LobbyCharacterMap.PreferredIndex(mapped, 0));
        Assert.Equal(0, LobbyCharacterMap.PreferredIndex(mapped, 4));
        Assert.Equal(1, LobbyCharacterMap.PreferredIndex(mapped, 9));
        Assert.True(mapped[1].PendingDelete);
        Assert.False(LobbyCharacterText.CanEnter(mapped[1]));
        Assert.True(LobbyCharacterText.CanEnter(mapped[0]));
        Assert.Equal("Human Mystic", LobbyCharacterText.ClassLabel(10));
        Assert.Equal("Human Fighter", LobbyCharacterText.ClassLabel(0));
        Assert.Equal("Deletes in 1d 1h", LobbyCharacterText.DeleteCountdown(90061));
    }

    [Fact]
    public void Short_summary_and_an_extra_key_still_decode()
    {
        var shortList = new ShortCharacterListMsg
        {
            Characters = [new ShortCharacterSummaryMsg { CharacterId = 3, Name = "Old", ClassId = 0, Race = 0, Sex = 0, Level = 1 }]
        };
        Assert.True(ZoneV2Messages.TryDecodeCharacterList(Pack(shortList), out ZoneV2CharacterSummary[] shortRows));
        Assert.Equal("Old", shortRows[0].Name);
        Assert.Equal(0, shortRows[0].Slot);
        Assert.Equal(0, shortRows[0].HairStyle);
        Assert.False(shortRows[0].IsLastPlayed);

        var extra = new ExtraCharacterListMsg
        {
            Characters =
            [
                new ExtraCharacterSummaryMsg
                {
                    CharacterId = 8,
                    Name = "New",
                    ClassId = 0,
                    Race = 0,
                    Sex = 0,
                    Level = 2,
                    Slot = 0,
                    HairStyle = 1,
                    HairColor = 1,
                    Face = 2,
                    X = 5,
                    Y = 6,
                    Z = 7,
                    DeleteSecondsLeft = 0,
                    IsLastPlayed = true,
                    Future = "ignored"
                }
            ]
        };
        Assert.True(ZoneV2Messages.TryDecodeCharacterList(Pack(extra), out ZoneV2CharacterSummary[] extraRows));
        Assert.Equal(1, extraRows[0].HairStyle);
        Assert.True(extraRows[0].IsLastPlayed);
        Assert.Equal(2, extraRows[0].Face);
    }

    [Fact]
    public void Select_delete_and_create_keep_the_server_key_order()
    {
        var selected = MessagePackSerializer.Deserialize<BackendSelectCharacterMsg>(
            ZoneV2Messages.EncodeSelectCharacter(42, "Aria", -1));
        Assert.Equal(42, selected.CharacterId);
        Assert.Equal("Aria", selected.Name);
        Assert.Equal(-1, selected.Slot);

        var bySlot = MessagePackSerializer.Deserialize<BackendSelectCharacterMsg>(
            ZoneV2Messages.EncodeSelectCharacter(0, "", 3));
        Assert.Equal(0, bySlot.CharacterId);
        Assert.Equal("", bySlot.Name);
        Assert.Equal(3, bySlot.Slot);

        var delete = MessagePackSerializer.Deserialize<BackendDeleteCharacterMsg>(
            ZoneV2Messages.EncodeDeleteCharacter(-1, 42));
        Assert.Equal(-1, delete.Slot);
        Assert.Equal(42, delete.CharacterId);
        var restore = MessagePackSerializer.Deserialize<BackendDeleteCharacterMsg>(
            ZoneV2Messages.EncodeRestoreCharacter(2, 0));
        Assert.Equal(2, restore.Slot);
        Assert.Equal(0, restore.CharacterId);

        var create = MessagePackSerializer.Deserialize<BackendCreateCharacterMsg>(
            ZoneV2Messages.EncodeCreateCharacter("Aria", 10, 0, 1, 6, 2, 1));
        Assert.Equal("Aria", create.Name);
        Assert.Equal(10, create.ClassId);
        Assert.Equal(0, create.Race);
        Assert.Equal(1, create.Sex);
        Assert.Equal(6, create.HairStyle);
        Assert.Equal(2, create.HairColor);
        Assert.Equal(1, create.Face);
    }

    [Fact]
    public void Handoff_created_and_delete_replies_decode()
    {
        var handoff = new BackendHandoffMsg
        {
            Account = "hero",
            CharacterName = "Aria",
            CharacterId = 4,
            ClassId = 10,
            Token = "fa1.ticket",
            ZoneHost = "game.fineage.evpoint.by",
            ZonePort = 7777,
            ExpiresUnixSeconds = 1_800_000_000L
        };
        Assert.True(ZoneV2Messages.TryDecodeHandoff(Pack(handoff), out ZoneV2Handoff ticket));
        Assert.Equal("fa1.ticket", ticket.Token);
        Assert.Equal("game.fineage.evpoint.by", ticket.ZoneHost);
        Assert.Equal(7777, ticket.ZonePort);
        Assert.Equal(1_800_000_000L, ticket.ExpiresUnixSeconds);

        byte[] zoneAuth = ZoneV2Messages.EncodeZoneAuth(ticket);
        var auth = MessagePackSerializer.Deserialize<BackendAuthMsg>(zoneAuth);
        Assert.Equal("hero", auth.Account);
        Assert.Equal("fa1.ticket", auth.Password);
        byte[] leaked = ZoneV2Messages.EncodeAuth("hero", "secret-password");
        Assert.NotEqual(leaked, zoneAuth);

        var created = new BackendCharacterCreatedMsg
        {
            CharacterId = 11,
            Name = "Aria",
            ClassId = 0,
            Race = 0,
            Sex = 0,
            HairStyle = 4,
            HairColor = 3,
            Face = 2,
            X = -84300,
            Y = 244600,
            Z = -3730
        };
        Assert.True(ZoneV2Messages.TryDecodeCharacterCreated(Pack(created), out ZoneV2CharacterCreated made));
        Assert.Equal(11, made.CharacterId);
        Assert.Equal(4, made.HairStyle);
        Assert.Equal(-84300, made.X);

        var deleted = new BackendCharacterDeletedMsg { Slot = 1, CharacterId = 9, HardDeleted = false };
        Assert.True(ZoneV2Messages.TryDecodeCharacterDeleted(Pack(deleted), out ZoneV2CharacterDeleted gone));
        Assert.False(gone.HardDeleted);
        Assert.Equal(9, gone.CharacterId);

        var failed = new BackendCharacterDeleteFailedMsg
        {
            Slot = 1,
            CharacterId = 9,
            Code = "character_not_found",
            Message = "missing"
        };
        Assert.True(ZoneV2Messages.TryDecodeCharacterDeleteFailed(Pack(failed), out ZoneV2CharacterDeleteFailed reject));
        Assert.Equal("character_not_found", reject.Code);
        Assert.Equal("That character was not found.", LobbyCharacterText.ErrorMessage(reject.Code, reject.Message));
    }

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
        LobbyCharacter[] empty = LobbyCharacterMap.FromSummaries(Array.Empty<ZoneV2CharacterSummary>());
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

    static byte[] Pack<T>(T message)
    {
        return MessagePackSerializer.Serialize(message);
    }
}

[MessagePackObject]
public sealed partial class BackendCharacterSummaryMsg
{
    [Key(0)] public int CharacterId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int ClassId { get; set; }
    [Key(3)] public int Race { get; set; }
    [Key(4)] public int Sex { get; set; }
    [Key(5)] public int Level { get; set; }
    [Key(6)] public int Slot { get; set; }
    [Key(7)] public int HairStyle { get; set; }
    [Key(8)] public int HairColor { get; set; }
    [Key(9)] public int Face { get; set; }
    [Key(10)] public int X { get; set; }
    [Key(11)] public int Y { get; set; }
    [Key(12)] public int Z { get; set; }
    [Key(13)] public int DeleteSecondsLeft { get; set; }
    [Key(14)] public bool IsLastPlayed { get; set; }
}

[MessagePackObject]
public sealed partial class BackendCharacterListMsg
{
    [Key(0)] public BackendCharacterSummaryMsg[] Characters { get; set; } = [];
}

[MessagePackObject]
public sealed partial class ShortCharacterSummaryMsg
{
    [Key(0)] public int CharacterId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int ClassId { get; set; }
    [Key(3)] public int Race { get; set; }
    [Key(4)] public int Sex { get; set; }
    [Key(5)] public int Level { get; set; }
}

[MessagePackObject]
public sealed partial class ShortCharacterListMsg
{
    [Key(0)] public ShortCharacterSummaryMsg[] Characters { get; set; } = [];
}

[MessagePackObject]
public sealed partial class ExtraCharacterSummaryMsg
{
    [Key(0)] public int CharacterId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int ClassId { get; set; }
    [Key(3)] public int Race { get; set; }
    [Key(4)] public int Sex { get; set; }
    [Key(5)] public int Level { get; set; }
    [Key(6)] public int Slot { get; set; }
    [Key(7)] public int HairStyle { get; set; }
    [Key(8)] public int HairColor { get; set; }
    [Key(9)] public int Face { get; set; }
    [Key(10)] public int X { get; set; }
    [Key(11)] public int Y { get; set; }
    [Key(12)] public int Z { get; set; }
    [Key(13)] public int DeleteSecondsLeft { get; set; }
    [Key(14)] public bool IsLastPlayed { get; set; }
    [Key(15)] public string Future { get; set; } = "";
}

[MessagePackObject]
public sealed partial class ExtraCharacterListMsg
{
    [Key(0)] public ExtraCharacterSummaryMsg[] Characters { get; set; } = [];
}

[MessagePackObject]
public sealed partial class BackendSelectCharacterMsg
{
    [Key(0)] public int CharacterId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int Slot { get; set; } = -1;
}

[MessagePackObject]
public sealed partial class BackendDeleteCharacterMsg
{
    [Key(0)] public int Slot { get; set; }
    [Key(1)] public int CharacterId { get; set; }
}

[MessagePackObject]
public sealed partial class BackendCreateCharacterMsg
{
    [Key(0)] public string Name { get; set; } = "";
    [Key(1)] public int ClassId { get; set; }
    [Key(2)] public int Race { get; set; }
    [Key(3)] public int Sex { get; set; }
    [Key(4)] public int HairStyle { get; set; }
    [Key(5)] public int HairColor { get; set; }
    [Key(6)] public int Face { get; set; }
}

[MessagePackObject]
public sealed partial class BackendHandoffMsg
{
    [Key(0)] public string Account { get; set; } = "";
    [Key(1)] public string CharacterName { get; set; } = "";
    [Key(2)] public int CharacterId { get; set; }
    [Key(3)] public int ClassId { get; set; }
    [Key(4)] public string Token { get; set; } = "";
    [Key(5)] public string ZoneHost { get; set; } = "";
    [Key(6)] public int ZonePort { get; set; }
    [Key(7)] public long ExpiresUnixSeconds { get; set; }
}

[MessagePackObject]
public sealed partial class BackendAuthMsg
{
    [Key(0)] public string Account { get; set; } = "";
    [Key(1)] public string Password { get; set; } = "";
}

[MessagePackObject]
public sealed partial class BackendCharacterCreatedMsg
{
    [Key(0)] public int CharacterId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int ClassId { get; set; }
    [Key(3)] public int Race { get; set; }
    [Key(4)] public int Sex { get; set; }
    [Key(5)] public int HairStyle { get; set; }
    [Key(6)] public int HairColor { get; set; }
    [Key(7)] public int Face { get; set; }
    [Key(8)] public int X { get; set; }
    [Key(9)] public int Y { get; set; }
    [Key(10)] public int Z { get; set; }
}

[MessagePackObject]
public sealed partial class BackendCharacterDeletedMsg
{
    [Key(0)] public int Slot { get; set; }
    [Key(1)] public int CharacterId { get; set; }
    [Key(2)] public bool HardDeleted { get; set; }
}

[MessagePackObject]
public sealed partial class BackendCharacterDeleteFailedMsg
{
    [Key(0)] public int Slot { get; set; }
    [Key(1)] public int CharacterId { get; set; }
    [Key(2)] public string Code { get; set; } = "";
    [Key(3)] public string Message { get; set; } = "";
}
