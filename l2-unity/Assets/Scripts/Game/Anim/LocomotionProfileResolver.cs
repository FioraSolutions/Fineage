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

    public static LocomotionProfile ResolveFromGear(UserGear gear)
    {
        if (gear == null)
        {
            FineageLog.Error(FineageLogCategory.World, "LocomotionProfile: missing UserGear on remote entity");
            return LocomotionProfile.BareHand;
        }

        CombatStyleProfile combat = gear.CombatStyle;
        if (combat.IsResolved && !string.IsNullOrEmpty(combat.AnimSuffix))
            return new LocomotionProfile(combat.AnimSuffix, LobbyStandardWalkSpeed, isResolved: true);

        FineageLog.Error(
            FineageLogCategory.World,
            "LocomotionProfile: unresolved CombatStyle on UserGear entity=" + gear.gameObject.name);
        return LocomotionProfile.BareHand;
    }

    public static string RemoteAnimatorSuffix(UserGear gear) => ResolveFromGear(gear).AnimSuffix;

    public static float LobbyWalkSpeed(CharacterRaceAnimation raceAnimation)
    {
        return LobbyStandardWalkRaces.Contains(raceAnimation)
            ? LobbyStandardWalkSpeed
            : LobbyAltWalkSpeed;
    }
}