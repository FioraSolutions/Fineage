/// <summary>
/// Appearance-bound race animation resolve kept in Assembly-CSharp (#131).
/// Enums / pure parsers live in <c>Fineage.Game</c>.
/// </summary>
public static class CharacterRaceAnimationResolve
{
    /// <summary>
    /// L2 body mesh is fighter vs mystic. CharInfo sends a real class id (Wizard=11, …);
    /// CharSelected leaves <see cref="PlayerAppearance.BaseClass"/> at 0, but fills
    /// <see cref="EntityIdentity.PlayerClass"/>.
    /// </summary>
    public static CharacterRaceAnimation ResolveRaceAnimation(
        CharacterRace race,
        PlayerAppearance appearance,
        int playerClass)
    {
        int sex = appearance != null ? appearance.Sex : 0;
        int baseClass = appearance != null ? appearance.BaseClass : (int)BaseClass.Fighter;
        bool isMage = baseClass == (int)BaseClass.MMagic ||
                      CharacterClassParser.IsMage(playerClass);
        int resolved = isMage ? (int)BaseClass.MMagic : (int)BaseClass.Fighter;
        if (appearance != null)
            appearance.BaseClass = resolved;
        return CharacterRaceAnimationParser.ParseRaceInterlude(race, sex, resolved);
    }
}
