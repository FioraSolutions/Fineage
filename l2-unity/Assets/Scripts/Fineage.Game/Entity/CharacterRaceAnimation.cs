using System;
using UnityEngine;

public enum CharacterRaceAnimation : byte
{
    FElf = 0,
    MElf = 1,
    MDarkElf = 2,
    FDarkElf = 3,
    FDwarf = 4,
    MDwarf = 5,
    MShaman = 6,
    FShaman = 7,
    MOrc = 8,
    FOrc = 9,
    FMagic = 10,
    MMagic = 11,
    FFighter = 12,
    MFighter = 13
}

public enum BaseClass : int
{
    Fighter = InterludeCharAppearance.Fighter,
    MMagic = InterludeCharAppearance.Mystic,
}

public static class CharacterRaceAnimationParser
{
    public static CharacterRaceAnimation ParseRace(CharacterRace race, byte sex, bool isMage)
    {
        if (CharacterRaceAnimationTable.TryResolve(race, sex, isMage, out CharacterRaceAnimation anim))
            return anim;

        int archetype = isMage ? (int)BaseClass.MMagic : (int)BaseClass.Fighter;
        CharacterRaceAnimationTable.LogMissing(race, sex, archetype);
        return CharacterRaceAnimation.FDwarf;
    }

    public static CharacterRaceAnimation SafeConvertToRace(int value)
    {
        try
        {
            byte byteValue = Convert.ToByte(value);
            if (Enum.IsDefined(typeof(CharacterRaceAnimation), byteValue))
                return (CharacterRaceAnimation)byteValue;
        }
        catch (OverflowException ex)
        {
            Debug.LogError(
                CharacterRaceAnimationTable.LogTag + " SafeConvertToRace overflow value=" + value + ": " + ex.Message);
        }

        Debug.LogError(
            CharacterRaceAnimationTable.LogTag + " undefined animation id=" + value
            + " (wire value is not a valid CharacterRaceAnimation).");
        return CharacterRaceAnimation.FElf;
    }

    public static CharacterRaceAnimation ParseRaceInterlude(CharacterRace race, int sex, int baseClass)
    {
        if (CharacterRaceAnimationTable.TryResolveInterlude(race, sex, baseClass, out CharacterRaceAnimation anim))
            return anim;

        CharacterRaceAnimationTable.LogMissing(race, sex, baseClass);
        return CharacterRaceAnimation.FDwarf;
    }
}