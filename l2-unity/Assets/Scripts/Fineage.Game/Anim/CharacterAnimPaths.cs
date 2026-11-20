using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resources layout for race animation clips (Fighter/Magic/Elf/… packs).
/// Prefab containers live under CharacterRace folders (Human/…); clips do not.
/// </summary>
public static class CharacterAnimPaths
{
    /// <summary>Lobby select-row walk multiplier. One rate — clip length already differs per race.</summary>
    public const float LobbyWalkSpeed = 0.5f;

    static readonly Dictionary<CharacterRaceAnimation, string> ClipPackByAnimation = new()
    {
        { CharacterRaceAnimation.MFighter, "Fighter" },
        { CharacterRaceAnimation.FFighter, "Fighter" },
        { CharacterRaceAnimation.MMagic, "Magic" },
        { CharacterRaceAnimation.FMagic, "Magic" },
        { CharacterRaceAnimation.MElf, "Elf" },
        { CharacterRaceAnimation.FElf, "Elf" },
        { CharacterRaceAnimation.MDarkElf, "DarkElf" },
        { CharacterRaceAnimation.FDarkElf, "DarkElf" },
        { CharacterRaceAnimation.MDwarf, "Dwarf" },
        { CharacterRaceAnimation.FDwarf, "Dwarf" },
        { CharacterRaceAnimation.MOrc, "Orc" },
        { CharacterRaceAnimation.FOrc, "Orc" },
        { CharacterRaceAnimation.MShaman, "Orc" },
        { CharacterRaceAnimation.FShaman, "Orc" },
    };

    public static string ClipPack(CharacterRaceAnimation raceAnim)
    {
        if (ClipPackByAnimation.TryGetValue(raceAnim, out string pack))
            return pack;

        Debug.LogError(
            CharacterRaceAnimationTable.LogTag + " ClipPack undefined animation profile=" + raceAnim
            + "; deriving folder name from CharacterRace.");
        return CharacterRaceParser.ParseRace(raceAnim).ToString();
    }

    public static string ClipsFolder(CharacterRaceAnimation raceAnim)
    {
        string race = raceAnim.ToString();
        return "Data/Animations/" + ClipPack(raceAnim) + "/" + race + "/Clips/";
    }
}