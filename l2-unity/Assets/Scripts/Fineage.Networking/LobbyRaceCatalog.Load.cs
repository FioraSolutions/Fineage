using System;
using System.Collections.Generic;

public static partial class LobbyRaceCatalog
{
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
}
