using System.Collections.Generic;

/// <summary>
/// Maps attach sockets to skeleton bone names. Race-specific overrides live here as data,
/// not as branches in Gear.
/// </summary>
public static class WeaponSkeletonSocketMap
{
    static readonly Dictionary<WeaponAttachSocket, string> DefaultBoneNames = new Dictionary<WeaponAttachSocket, string>
    {
        { WeaponAttachSocket.Shield, "Shield_L_Bone" },
        { WeaponAttachSocket.Bow, "Weapon_L_Bone" },
        { WeaponAttachSocket.LeftHand, "Weapon_L_Bone" },
        { WeaponAttachSocket.RightHand, "Weapon_R_Bone" },
    };

    // Optional per-skeleton overrides (skeleton id -> socket -> bone name).
    static readonly Dictionary<string, Dictionary<WeaponAttachSocket, string>> Overrides =
        new Dictionary<string, Dictionary<WeaponAttachSocket, string>>();

    public static string GetBoneName(WeaponAttachSocket socket, string skeletonId = null)
    {
        if (!string.IsNullOrEmpty(skeletonId)
            && Overrides.TryGetValue(skeletonId, out var perSkeleton)
            && perSkeleton.TryGetValue(socket, out string overrideName)
            && !string.IsNullOrEmpty(overrideName))
        {
            return overrideName;
        }

        return DefaultBoneNames[socket];
    }

    public static void RegisterOverride(string skeletonId, WeaponAttachSocket socket, string boneName)
    {
        if (string.IsNullOrEmpty(skeletonId) || string.IsNullOrEmpty(boneName))
            return;
        if (!Overrides.TryGetValue(skeletonId, out var map))
        {
            map = new Dictionary<WeaponAttachSocket, string>();
            Overrides[skeletonId] = map;
        }

        map[socket] = boneName;
    }
}