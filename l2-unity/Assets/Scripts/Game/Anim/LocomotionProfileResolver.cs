using System.Collections.Generic;

public static class LocomotionProfileResolver
{
    public static LocomotionProfile ResolveFromGear(UserGear gear)
    {
        if (gear == null)
        {
            FineageLog.Error(FineageLogCategory.World, "LocomotionProfile: missing UserGear on remote entity");
            return LocomotionProfile.BareHand;
        }

        CombatStyleProfile combat = gear.CombatStyle;
        if (combat.IsResolved && !string.IsNullOrEmpty(combat.AnimSuffix))
            return new LocomotionProfile(combat.AnimSuffix, LocomotionProfileLobby.LobbyWalkSpeed(CharacterRaceAnimation.FFighter), isResolved: true);

        FineageLog.Error(
            FineageLogCategory.World,
            "LocomotionProfile: unresolved CombatStyle on UserGear entity=" + gear.gameObject.name);
        return LocomotionProfile.BareHand;
    }

    public static string RemoteAnimatorSuffix(UserGear gear) => ResolveFromGear(gear).AnimSuffix;

    public static float LobbyWalkSpeed(CharacterRaceAnimation raceAnimation) =>
        LocomotionProfileLobby.LobbyWalkSpeed(raceAnimation);
}