using System.Collections.Generic;

public static class LocomotionProfileResolver
{
    static readonly HashSet<CharacterRaceAnimation> LobbyStandardWalkRaces = new HashSet<CharacterRaceAnimation>
    {
        CharacterRaceAnimation.FFighter,
        CharacterRaceAnimation.MFighter,
        CharacterRaceAnimation.FMagic,
        CharacterRaceAnimation.MMagic,
    };

    const float LobbyStandardWalkSpeed = 1.2f;
    const float LobbyAltWalkSpeed = 1.5f;

    public static LocomotionProfile ResolveFromCombatStyle(CombatStyleProfile combat, string entityLabel = null)
    {
        if (combat.IsResolved && !string.IsNullOrEmpty(combat.AnimSuffix))
            return new LocomotionProfile(combat.AnimSuffix, LobbyStandardWalkSpeed, isResolved: true);

        FineageLog.Error(
            FineageLogCategory.World,
            "LocomotionProfile: unresolved CombatStyle" +
            (string.IsNullOrEmpty(entityLabel) ? "" : " entity=" + entityLabel));
        return LocomotionProfile.BareHand;
    }

    public static string RemoteAnimatorSuffix(CombatStyleProfile combat, string entityLabel = null) =>
        ResolveFromCombatStyle(combat, entityLabel).AnimSuffix;

    public static float LobbyWalkSpeed(CharacterRaceAnimation raceAnimation)
    {
        return LobbyStandardWalkRaces.Contains(raceAnimation)
            ? LobbyStandardWalkSpeed
            : LobbyAltWalkSpeed;
    }
}