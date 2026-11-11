using System;
using System.Collections.Generic;


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
