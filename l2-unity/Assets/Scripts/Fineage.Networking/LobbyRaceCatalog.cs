using System;
using System.Collections.Generic;

public static partial class LobbyRaceCatalog
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

}
