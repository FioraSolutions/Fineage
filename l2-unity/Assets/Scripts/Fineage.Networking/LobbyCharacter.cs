using System;
using System.Collections.Generic;

/// <summary>
/// Character-select row for the lobby UI (presentation shape, not a raw Net contract row).
/// </summary>
public sealed class LobbyCharacter
{
    public int Id;
    public string Name;
    public int ClassId;
    public int Race;
    public int Sex;
    public int Level;
    public int Slot;
    public int HairStyle;
    public int HairColor;
    public int Face;
    public int X;
    public int Y;
    public int Z;
    public int DeleteSecondsLeft;
    public bool IsLastPlayed;
    public int CurHp, MaxHp, CurMp, MaxMp, CurCp, MaxCp;
    public long Exp;
    public int Sp, Karma, WeaponItemId;
    /// <summary>Equipped item ids by paperdoll slot (length 17). Null when login omitted key 25.</summary>
    public int[] Paperdoll;

    public bool PendingDelete
    {
        get { return DeleteSecondsLeft > 0; }
    }
}

public static class LobbyCharacterMap
{
    public static LobbyCharacter FromSummary(NetCharacterSummary row)
    {
        return new LobbyCharacter
        {
            Id = row.CharacterId,
            Name = row.Name ?? "",
            ClassId = row.ClassId,
            Race = row.Race,
            Sex = row.Sex,
            Level = row.Level,
            Slot = row.Slot,
            HairStyle = row.HairStyle,
            HairColor = row.HairColor,
            Face = row.Face,
            X = row.X,
            Y = row.Y,
            Z = row.Z,
            DeleteSecondsLeft = row.DeleteSecondsLeft,
            IsLastPlayed = row.IsLastPlayed,
            CurHp = row.CurHp,
            MaxHp = row.MaxHp,
            CurMp = row.CurMp,
            MaxMp = row.MaxMp,
            CurCp = row.CurCp,
            MaxCp = row.MaxCp,
            Exp = row.Exp,
            Sp = row.Sp,
            Karma = row.Karma,
            WeaponItemId = row.WeaponItemId,
            Paperdoll = row.Paperdoll
        };
    }

    public static LobbyCharacter[] FromSummaries(NetCharacterSummary[] rows)
    {
        if (rows == null || rows.Length == 0)
            return Array.Empty<LobbyCharacter>();
        LobbyCharacter[] list = new LobbyCharacter[rows.Length];
        for (int i = 0; i < rows.Length; i++)
            list[i] = FromSummary(rows[i]);
        return list;
    }

    public static LobbyCharacter FromNetSummary(Fineage.Net.Contract.CharacterSummaryNet row)
    {
        int deleteSeconds = row.DeleteSecondsLeft;
        if (deleteSeconds <= 0 && row.PendingDelete)
            deleteSeconds = 1;
        return new LobbyCharacter
        {
            Id = row.CharacterId,
            Name = row.Name ?? "",
            ClassId = row.ClassId,
            Race = row.Race,
            Sex = row.Sex,
            Level = row.Level,
            Slot = row.Slot,
            HairStyle = row.HairStyle,
            HairColor = row.HairColor,
            Face = row.Face,
            X = row.X,
            Y = row.Y,
            Z = row.Z,
            DeleteSecondsLeft = deleteSeconds,
            IsLastPlayed = row.IsLastPlayed,
            CurHp = row.Hp,
            MaxHp = row.MaxHp,
            CurMp = row.Mp,
            MaxMp = row.MaxMp,
            CurCp = row.CurCp,
            MaxCp = row.MaxCp,
            Exp = row.Exp,
            Sp = row.Sp,
            Karma = row.Karma,
            WeaponItemId = row.WeaponItemId,
            Paperdoll = row.Paperdoll
        };
    }

    public static LobbyCharacter[] FromNetRoster(Fineage.Net.Contract.CharacterRosterMsg roster)
    {
        if (roster?.Characters == null || roster.Characters.Length == 0)
            return Array.Empty<LobbyCharacter>();
        LobbyCharacter[] list = new LobbyCharacter[roster.Characters.Length];
        for (int i = 0; i < roster.Characters.Length; i++)
            list[i] = FromNetSummary(roster.Characters[i]);
        return list;
    }

    /// <summary>New character id, otherwise the last-played row, otherwise the first slot.</summary>
    public static int PreferredIndex(IReadOnlyList<LobbyCharacter> rows, int preselectId)
    {
        if (rows == null || rows.Count == 0)
            return -1;
        if (preselectId > 0)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i] != null && rows[i].Id == preselectId)
                    return i;
            }
        }

        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i] != null && rows[i].IsLastPlayed)
                return i;
        }

        return 0;
    }
}

/// <summary>Labels and lobby rules shared by the select screen. The server stays authoritative.</summary>
public static class LobbyCharacterText
{
    public const int CharMaxNumber = 7;
    public const string EmptyRosterStatus = "No characters yet.";
    public const int RaceHuman = 0;
    public const int ClassFighter = 0;
    public const int ClassMystic = 10;

    public static bool CanCreate(int characterCount)
    {
        return characterCount < CharMaxNumber;
    }

    public static bool CanEnter(LobbyCharacter character)
    {
        return character != null && !character.PendingDelete;
    }

    public static string ClassLabel(int classId)
    {
        string label = LobbyRaceCatalog.ClassLabel(classId);
        if (label != null)
            return label;
        return "Class " + classId;
    }

    public static string DeleteCountdown(int seconds)
    {
        if (seconds <= 0)
            return "";
        int days = seconds / 86400;
        int hours = (seconds % 86400) / 3600;
        int minutes = (seconds % 3600) / 60;
        if (days > 0)
            return "Deletes in " + days + "d " + hours + "h";
        if (hours > 0)
            return "Deletes in " + hours + "h " + minutes + "m";
        if (minutes > 0)
            return "Deletes in " + minutes + "m";
        return "Deletes in " + seconds + "s";
    }

    public static string ErrorMessage(string code, string serverMessage)
    {
        switch (code)
        {
            case "name_too_long":
                return "Maximum 16 letters or digits.";
            case "invalid_name":
                return "Incorrect name.";
            case "name_taken":
                return "This name already exists.";
            case "character_limit":
                return "You cannot create another character. Delete one first.";
            case "creation_failed":
            case "race_not_supported":
            case "class_not_supported":
            case "sex_not_supported":
                return "Character creation failed.";
            case "create_not_allowed":
                return "Characters cannot be created on this server.";
            case "character_not_found":
                return "That character was not found.";
            case "character_pending_delete":
                return "That character is pending deletion.";
            case "delete_failed":
                return "Could not delete that character.";
            case "auth_required":
                return "Login required.";
            case "bad_request":
                return "The request was rejected.";
            case "bad_slot":
                return "That character slot is not valid.";
            default:
                if (!string.IsNullOrEmpty(serverMessage))
                    return serverMessage;
                if (!string.IsNullOrEmpty(code))
                    return code;
                return "The server rejected the request.";
        }
    }
}

/// <summary>
/// Client-side create checks for the five Interlude races. The server stays authoritative.
/// Appearance ranges match Fineage-Backend: hair 0–4 male / 0–6 female, color 0–3, face 0–2.
/// </summary>
public static class LobbyCharacterCreate
{
    public const int SexMale = 0;
    public const int SexFemale = 1;

    public static readonly string[] ForbiddenFragments =
    {
        "annou", "ammou", "amnou", "anmou", "anou", "amou"
    };

    public static readonly string[] MaleHair =
    {
        "Type A", "Type B", "Type C", "Type D", "Type E"
    };

    public static readonly string[] FemaleHair =
    {
        "Type A", "Type B", "Type C", "Type D", "Type E", "Type F", "Type G"
    };

    public static int MaxHairStyle(int sex)
    {
        return sex == SexFemale ? 6 : 4;
    }

    public static string[] HairLabels(int sex)
    {
        return sex == SexFemale ? FemaleHair : MaleHair;
    }

    public static int ClampAppearance(int value, int max)
    {
        if (value < 0)
            return 0;
        if (value > max)
            return max;
        return value;
    }

    /// <summary>Returns a server error code, or null when the request is allowed.</summary>
    public static string Validate(string name, int classId, int race, int sex, int hairStyle, int hairColor, int face)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 16)
            return "name_too_long";
        name = name.Trim();
        if (!IsAsciiAlnum(name) || ContainsForbidden(name))
            return "invalid_name";
        if (sex != SexMale && sex != SexFemale)
            return "sex_not_supported";
        if (face < 0 || face > 2 || hairColor < 0 || hairColor > 3)
            return "creation_failed";
        if (hairStyle < 0 || hairStyle > MaxHairStyle(sex))
            return "creation_failed";
        if (!LobbyRaceCatalog.IsBaseClass(classId))
            return "class_not_supported";
        if (!LobbyRaceCatalog.RaceMatchesClass(classId, race))
            return "race_not_supported";
        return null;
    }

    static bool ContainsForbidden(string name)
    {
        for (int i = 0; i < ForbiddenFragments.Length; i++)
        {
            if (name.IndexOf(ForbiddenFragments[i], StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        }

        return false;
    }

    static bool IsAsciiAlnum(string name)
    {
        for (int i = 0; i < name.Length; i++)
        {
            char ch = name[i];
            bool digit = ch >= '0' && ch <= '9';
            bool upper = ch >= 'A' && ch <= 'Z';
            bool lower = ch >= 'a' && ch <= 'z';
            if (!digit && !upper && !lower)
                return false;
        }

        return name.Length > 0;
    }
}

/// <summary>
/// Interlude base races and classes the character-create UI can send.
/// Race ids are 0 Human, 1 Elf, 2 Dark Elf, 3 Orc, 4 Dwarf.
/// Class ids are the Interlude base classes (Human Fighter 0, Orc Fighter 44, Dwarf Fighter 53, …).
/// </summary>
public static class LobbyRaceCatalog
{
    public static readonly string[] RaceNames = { "Human", "Elf", "Dark Elf", "Orc", "Dwarf" };
    public static readonly string[] FighterAndMystic = { "Fighter", "Mystic" };
    public static readonly string[] FighterOnly = { "Fighter" };

    public const int HumanFighter = 0;
    public const int HumanMystic = 10;
    public const int ElfFighter = 18;
    public const int ElfMystic = 25;
    public const int DarkElfFighter = 31;
    public const int DarkElfMystic = 38;
    public const int OrcFighter = 44;
    public const int OrcMystic = 49;
    public const int DwarfFighter = 53;

    public static string[] ClassNames(string raceName)
    {
        if (raceName == "Dwarf")
            return FighterOnly;
        return FighterAndMystic;
    }

    public static int RaceId(string raceName)
    {
        switch (raceName)
        {
            case "Human": return 0;
            case "Elf": return 1;
            case "Dark Elf": return 2;
            case "Orc": return 3;
            case "Dwarf": return 4;
            default: return -1;
        }
    }

    public static int ClassIdFor(string raceName, string className)
    {
        bool fighter = className == "Fighter";
        bool mystic = className == "Mystic";
        switch (raceName)
        {
            case "Human":
                if (fighter) return HumanFighter;
                if (mystic) return HumanMystic;
                return -1;
            case "Elf":
                if (fighter) return ElfFighter;
                if (mystic) return ElfMystic;
                return -1;
            case "Dark Elf":
                if (fighter) return DarkElfFighter;
                if (mystic) return DarkElfMystic;
                return -1;
            case "Orc":
                if (fighter) return OrcFighter;
                if (mystic) return OrcMystic;
                return -1;
            case "Dwarf":
                if (fighter) return DwarfFighter;
                return -1;
            default:
                return -1;
        }
    }

    public static bool IsBaseClass(int classId)
    {
        switch (classId)
        {
            case HumanFighter:
            case HumanMystic:
            case ElfFighter:
            case ElfMystic:
            case DarkElfFighter:
            case DarkElfMystic:
            case OrcFighter:
            case OrcMystic:
            case DwarfFighter:
                return true;
            default:
                return false;
        }
    }

    public static int RaceForClass(int classId)
    {
        // Full Interlude trees including 3rd profession (not starters only).
        if ((classId >= 0 && classId <= 17) || (classId >= 88 && classId <= 98))
            return 0; // Human
        if ((classId >= 18 && classId <= 30) || (classId >= 99 && classId <= 105))
            return 1; // Elf
        if ((classId >= 31 && classId <= 43) || (classId >= 106 && classId <= 112))
            return 2; // Dark Elf
        if ((classId >= 44 && classId <= 52) || (classId >= 113 && classId <= 116))
            return 3; // Orc
        if ((classId >= 53 && classId <= 57) || (classId >= 117 && classId <= 118))
            return 4; // Dwarf
        return -1;
    }

    public static bool RaceMatchesClass(int classId, int race)
    {
        int expected = RaceForClass(classId);
        return expected >= 0 && expected == race;
    }

    private static readonly System.Collections.Generic.Dictionary<int, string> Labels = new System.Collections.Generic.Dictionary<int, string>
    {
        { 0, "Human Fighter" },
        { 1, "Warrior" },
        { 2, "Gladiator" },
        { 3, "Warlord" },
        { 4, "Human Knight" },
        { 5, "Paladin" },
        { 6, "Dark Avenger" },
        { 7, "Rogue" },
        { 8, "Treasure Hunter" },
        { 9, "Hawkeye" },
        { 10, "Human Mystic" },
        { 11, "Human Wizard" },
        { 12, "Sorcerer" },
        { 13, "Necromancer" },
        { 14, "Warlock" },
        { 15, "Cleric" },
        { 16, "Bishop" },
        { 17, "Prophet" },
        { 18, "Elven Fighter" },
        { 19, "Elven Knight" },
        { 20, "Temple Knight" },
        { 21, "Sword Singer" },
        { 22, "Elven Scout" },
        { 23, "Plains Walker" },
        { 24, "Silver Ranger" },
        { 25, "Elven Mystic" },
        { 26, "Elven Wizard" },
        { 27, "Spellsinger" },
        { 28, "Elemental Summoner" },
        { 29, "Elven Oracle" },
        { 30, "Elven Elder" },
        { 31, "Dark Fighter" },
        { 32, "Palus Knight" },
        { 33, "Shillien Knight" },
        { 34, "Bladedancer" },
        { 35, "Assassin" },
        { 36, "Abyss Walker" },
        { 37, "Phantom Ranger" },
        { 38, "Dark Mystic" },
        { 39, "Dark Wizard" },
        { 40, "Spellhowler" },
        { 41, "Phantom Summoner" },
        { 42, "Shillien Oracle" },
        { 43, "Shillien Elder" },
        { 44, "Orc Fighter" },
        { 45, "Orc Raider" },
        { 46, "Destroyer" },
        { 47, "Monk" },
        { 48, "Tyrant" },
        { 49, "Orc Mystic" },
        { 50, "Orc Shaman" },
        { 51, "Overlord" },
        { 52, "Warcryer" },
        { 53, "Dwarf Fighter" },
        { 54, "Scavenger" },
        { 55, "Bounty Hunter" },
        { 56, "Artisan" },
        { 57, "Warsmith" },
        { 88, "Duelist" },
        { 89, "Dreadnought" },
        { 90, "Phoenix Knight" },
        { 91, "Hell Knight" },
        { 92, "Sagittarius" },
        { 93, "Adventurer" },
        { 94, "Archmage" },
        { 95, "Soultaker" },
        { 96, "Arcana Lord" },
        { 97, "Cardinal" },
        { 98, "Hierophant" },
        { 99, "Eva's Templar" },
        { 100, "Sword Muse" },
        { 101, "Wind Rider" },
        { 102, "Moonlight Sentinel" },
        { 103, "Mystic Muse" },
        { 104, "Elemental Master" },
        { 105, "Eva's Saint" },
        { 106, "Shillien Templar" },
        { 107, "Spectral Dancer" },
        { 108, "Ghost Hunter" },
        { 109, "Ghost Sentinel" },
        { 110, "Storm Screamer" },
        { 111, "Spectral Master" },
        { 112, "Shillien Saint" },
        { 113, "Titan" },
        { 114, "Grand Khavatari" },
        { 115, "Dominator" },
        { 116, "Doom Cryer" },
        { 117, "Fortune Seeker" },
        { 118, "Maestro" },
    };

    public static string ClassLabel(int classId)
    {
        string name;
        return Labels.TryGetValue(classId, out name) ? name : null;
    }
}

