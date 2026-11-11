using System;
using System.Collections.Generic;

public static partial class LobbyRaceCatalog
{
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

    public static string ClassLabel(int classId)
    {
        string name;
        return Labels.TryGetValue(classId, out name) ? name : null;
    }
}
