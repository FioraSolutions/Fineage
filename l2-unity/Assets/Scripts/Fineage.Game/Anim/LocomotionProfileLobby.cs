using System.Collections.Generic;

/// <summary>Lobby walk speeds in Fineage.Game (asmdef-safe; resolver with UserGear stays in default assembly).</summary>
public static class LocomotionProfileLobby
{
    static readonly HashSet<CharacterRaceAnimation> StandardWalkRaces = new HashSet<CharacterRaceAnimation>
    {
        CharacterRaceAnimation.FFighter,
        CharacterRaceAnimation.MFighter,
        CharacterRaceAnimation.FMagic,
        CharacterRaceAnimation.MMagic,
    };

    const float StandardWalkSpeed = 1.2f;
    const float AltWalkSpeed = 1.5f;

    public static float LobbyWalkSpeed(CharacterRaceAnimation raceAnimation) =>
        StandardWalkRaces.Contains(raceAnimation) ? StandardWalkSpeed : AltWalkSpeed;
}