using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Shared race×sex×archetype → body animation profile (no per-race switch branches).</summary>
static class CharacterRaceAnimationTable
{
    public const string LogTag = "[CharacterRaceAnimation]";

    readonly struct Key : IEquatable<Key>
    {
        public readonly CharacterRace Race;
        public readonly int Sex;
        public readonly int Archetype;

        public Key(CharacterRace race, int sex, int archetype)
        {
            Race = race;
            Sex = sex;
            Archetype = archetype;
        }

        public bool Equals(Key other) =>
            Race == other.Race && Sex == other.Sex && Archetype == other.Archetype;

        public override bool Equals(object obj) => obj is Key other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Race, Sex, Archetype);
    }

    static readonly Dictionary<Key, CharacterRaceAnimation> ByKey = new();

    static CharacterRaceAnimationTable()
    {
        RegisterSexOnly(CharacterRace.Elf, CharacterRaceAnimation.MElf, CharacterRaceAnimation.FElf);
        RegisterSexOnly(CharacterRace.DarkElf, CharacterRaceAnimation.MDarkElf, CharacterRaceAnimation.FDarkElf);
        RegisterSexOnly(CharacterRace.Dwarf, CharacterRaceAnimation.MDwarf, CharacterRaceAnimation.FDwarf);

        RegisterArchetype(CharacterRace.Orc,
            fighterMale: CharacterRaceAnimation.MOrc,
            fighterFemale: CharacterRaceAnimation.FOrc,
            mysticMale: CharacterRaceAnimation.MShaman,
            mysticFemale: CharacterRaceAnimation.FShaman);

        RegisterArchetype(CharacterRace.Human,
            fighterMale: CharacterRaceAnimation.MFighter,
            fighterFemale: CharacterRaceAnimation.FFighter,
            mysticMale: CharacterRaceAnimation.MMagic,
            mysticFemale: CharacterRaceAnimation.FMagic);
    }

    static void RegisterSexOnly(CharacterRace race, CharacterRaceAnimation male, CharacterRaceAnimation female)
    {
        foreach (int archetype in new[] { (int)BaseClass.Fighter, (int)BaseClass.MMagic })
        {
            ByKey[new Key(race, 0, archetype)] = male;
            ByKey[new Key(race, 1, archetype)] = female;
        }
    }

    static void RegisterArchetype(
        CharacterRace race,
        CharacterRaceAnimation fighterMale,
        CharacterRaceAnimation fighterFemale,
        CharacterRaceAnimation mysticMale,
        CharacterRaceAnimation mysticFemale)
    {
        ByKey[new Key(race, 0, (int)BaseClass.Fighter)] = fighterMale;
        ByKey[new Key(race, 1, (int)BaseClass.Fighter)] = fighterFemale;
        ByKey[new Key(race, 0, (int)BaseClass.MMagic)] = mysticMale;
        ByKey[new Key(race, 1, (int)BaseClass.MMagic)] = mysticFemale;
    }

    static int NormalizeSex(int sex) => sex == 0 ? 0 : 1;

    static int ArchetypeFromMage(bool isMage) =>
        isMage ? (int)BaseClass.MMagic : (int)BaseClass.Fighter;

    static int NormalizeArchetype(int baseClass) =>
        baseClass == (int)BaseClass.MMagic ? (int)BaseClass.MMagic : (int)BaseClass.Fighter;

    public static bool TryResolve(CharacterRace race, int sex, bool isMage, out CharacterRaceAnimation anim)
    {
        var key = new Key(race, NormalizeSex(sex), ArchetypeFromMage(isMage));
        return ByKey.TryGetValue(key, out anim);
    }

    public static bool TryResolveInterlude(CharacterRace race, int sex, int baseClass, out CharacterRaceAnimation anim)
    {
        var key = new Key(race, NormalizeSex(sex), NormalizeArchetype(baseClass));
        return ByKey.TryGetValue(key, out anim);
    }

    public static void LogMissing(CharacterRace race, int sex, int archetype)
    {
        Debug.LogError(
            $"{LogTag} no animation profile race={race} sex={sex} archetype={archetype} "
            + "(expected Interlude body mesh row in CharacterRaceAnimationTable).");
    }
}