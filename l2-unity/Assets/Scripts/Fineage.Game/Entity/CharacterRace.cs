using System.Collections.Generic;
using UnityEngine;

public enum CharacterRace : byte
{
    Human = 0,
    Elf = 1,
    DarkElf = 2,
    Orc = 3,
    Dwarf = 4,
    Monster = 5
}

public static class CharacterRaceParser
{
    static readonly Dictionary<CharacterRaceAnimation, CharacterRace> AnimationToRace = new()
    {
        { CharacterRaceAnimation.FElf, CharacterRace.Elf },
        { CharacterRaceAnimation.MElf, CharacterRace.Elf },
        { CharacterRaceAnimation.MDarkElf, CharacterRace.DarkElf },
        { CharacterRaceAnimation.FDarkElf, CharacterRace.DarkElf },
        { CharacterRaceAnimation.FDwarf, CharacterRace.Dwarf },
        { CharacterRaceAnimation.MDwarf, CharacterRace.Dwarf },
        { CharacterRaceAnimation.MShaman, CharacterRace.Orc },
        { CharacterRaceAnimation.FShaman, CharacterRace.Orc },
        { CharacterRaceAnimation.MOrc, CharacterRace.Orc },
        { CharacterRaceAnimation.FOrc, CharacterRace.Orc },
        { CharacterRaceAnimation.FMagic, CharacterRace.Human },
        { CharacterRaceAnimation.MMagic, CharacterRace.Human },
        { CharacterRaceAnimation.FFighter, CharacterRace.Human },
        { CharacterRaceAnimation.MFighter, CharacterRace.Human },
    };

    public static CharacterRace ParseRace(CharacterRaceAnimation race)
    {
        if (AnimationToRace.TryGetValue(race, out CharacterRace parsed))
            return parsed;

        Debug.LogError(
            CharacterRaceAnimationTable.LogTag + " ParseRace unknown animation profile=" + race
            + " (no CharacterRace mapping).");
        return CharacterRace.Human;
    }
}