/// <summary>
/// Resources layout for race animation clips (Fighter/Magic/Elf/… packs).
/// Prefab containers live under CharacterRace folders (Human/…); clips do not.
/// </summary>
public static class CharacterAnimPaths
{
    /// <summary>Lobby select-row walk multiplier. One rate — clip length already differs per race.</summary>
    public const float LobbyWalkSpeed = 0.5f;

    public static string ClipPack(CharacterRaceAnimation raceAnim)
    {
        switch (raceAnim)
        {
            case CharacterRaceAnimation.MFighter:
            case CharacterRaceAnimation.FFighter:
                return "Fighter";
            case CharacterRaceAnimation.MMagic:
            case CharacterRaceAnimation.FMagic:
                return "Magic";
            case CharacterRaceAnimation.MElf:
            case CharacterRaceAnimation.FElf:
                return "Elf";
            case CharacterRaceAnimation.MDarkElf:
            case CharacterRaceAnimation.FDarkElf:
                return "DarkElf";
            case CharacterRaceAnimation.MDwarf:
            case CharacterRaceAnimation.FDwarf:
                return "Dwarf";
            case CharacterRaceAnimation.MOrc:
            case CharacterRaceAnimation.FOrc:
            case CharacterRaceAnimation.MShaman:
            case CharacterRaceAnimation.FShaman:
                return "Orc";
            default:
                return CharacterRaceParser.ParseRace(raceAnim).ToString();
        }
    }

    public static string ClipsFolder(CharacterRaceAnimation raceAnim)
    {
        string race = raceAnim.ToString();
        return "Data/Animations/" + ClipPack(raceAnim) + "/" + race + "/Clips/";
    }
}
