using UnityEngine;

public static partial class ItemDropPose
{
    public static bool TryBuildThrowPose(
        Component item,
        Abstractgrp grp,
        bool stickWeapon,
        Vector3 from,
        Vector3 landPos,
        out ThrowPose pose)
    {
        pose = default;
        if (item == null)
            return false;

        int dropType = grp != null ? grp.DropType : 0;
        Vector3 throwDir = landPos - from;

        pose.LandEuler = LandEulerFromDropType(dropType, throwDir, 0);
        pose.StickOnLand = stickWeapon &&
                           (dropType == 0 || (dropType >= 1 && dropType <= 4));
        pose.TumbleInFlight = pose.StickOnLand;
        pose.TumbleAxis = pose.TumbleInFlight
            ? TumbleWorldAxis(dropType, throwDir, 0)
            : Vector3.zero;
        pose.TumbleDegPerSec = pose.TumbleInFlight ? GetTumbleDegPerSec(0) : 0f;

        float horiz = Vector3.Distance(
            new Vector3(from.x, 0f, from.z),
            new Vector3(landPos.x, 0f, landPos.z));
        // OnDropItem: StartLoc.Z = landZ + Dist. Clamp so short drops still arc.
        pose.ArcHeightMeters = pose.StickOnLand
            ? Mathf.Clamp(horiz, 0.45f, 2.4f)
            : ItemDropPresentationIds.DropThrowArcHeightMeters;
        return true;
    }

    public static Vector3 LandEulerFromDropType(int dropType, Vector3 throwDir, int slot)
    {
        int pitch = 0;
        int yaw = dropType != 0 ? 0x4000 : 0;
        int roll = 0;
        int slotBias = (SlotYawStep * slot) / 360;

        switch (dropType)
        {
            case 1:
                pitch = -35498 - slotBias + 0x4000;
                break;
            case 2:
                pitch = -32768 - slotBias;
                break;
            case 3:
                pitch = -2731 - slotBias + 0x4000;
                break;
            case 4:
                pitch = slotBias + 0x4000;
                break;
        }

        Vector3 flat = new Vector3(throwDir.x, 0f, throwDir.z);
        if (flat.sqrMagnitude > 0.0001f)
        {
            float unityYaw = Quaternion.LookRotation(flat.normalized, Vector3.up).eulerAngles.y;
            int aimYaw = Mathf.RoundToInt(unityYaw * 65536f / 360f);
            yaw = dropType == 2 ? aimYaw + 0x4000 : aimYaw + 0x8000;
        }

        return VectorUtils.ConvertRotToUnity(new Vector3(pitch, yaw, roll));
    }
}
