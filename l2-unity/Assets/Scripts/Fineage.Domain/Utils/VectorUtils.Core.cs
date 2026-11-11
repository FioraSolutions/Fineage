using UnityEngine;

public static partial class VectorUtils
{
    public static Vector3 ConvertPosToUnity(Vector3 ueVector)
    {
        return new Vector3(ueVector.y, ueVector.z, ueVector.x) * (1f / 52.5f);
    }

    public static Vector3 ConvertPosUnityToL2j(Vector3 ueVector)
    {
        Vector3 convert = new Vector3(ueVector.y, ueVector.x, ueVector.z) / (1f / 52.5f);
        return new Vector3(convert.z, convert.y, convert.x);
    }

    public static void GetSwMapTile(Vector3 l2Pos, out int tileX, out int tileY)
    {
        tileX = Mathf.FloorToInt(l2Pos.x / SwTileSizeUu) + 20;
        tileY = Mathf.FloorToInt(l2Pos.y / SwTileSizeUu) + 18;
    }

    public static string FormatSwMapName(int tileX, int tileY)
    {
        return tileX.ToString("00") + "_" + tileY.ToString("00");
    }

    public static string GetSwMapName(Vector3 l2Pos)
    {
        GetSwMapTile(l2Pos, out int tileX, out int tileY);
        return FormatSwMapName(tileX, tileY);
    }

    public static Vector3 ConvertRotToUnity(Vector3 ueRot)
    {
        return new Vector3(
            -360.00f * ueRot.x / 65536,
            360.00f * ueRot.y / 65536,
            -360.00f * ueRot.z / 65536);
    }

    public static float ConvertL2jDistance(float distance)
    {
        return distance * 0.0165f;
    }

    public static float ConvertL2jDataOffset(float height)
    {
        return height / 0.5f;
    }

    public static float ConvertL2jDataOffsetRadius(float height)
    {
        return height;
    }

    public static bool IsTargetBehindPlayer(Vector3 targetPosition, Transform playerTransform)
    {
        Vector3 playerForward = playerTransform.forward;
        Vector3 toTarget = (targetPosition - playerTransform.position).normalized;
        float angle = Vector3.Angle(playerForward, toTarget);
        return angle > 90f;
    }

    public static bool IsFacingAttacker(Transform transform, Vector3 attackerPosition, float angleThreshold)
    {
        Vector3 directionToAttacker = (attackerPosition - transform.position).normalized;
        Vector3 forwardDirection = transform.forward;
        float angle = Vector3.Angle(forwardDirection, directionToAttacker);
        return angle < angleThreshold;
    }

    public static float InFaceProcent(Vector3 target, Transform transform, float maxAngle)
    {
        if (maxAngle <= 30) return 100;

        Vector3 targetVect = new Vector3(target.x, 0, target.z);
        Vector3 vectr = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 directionToTarget = (targetVect - vectr).normalized;

        float angle = Vector3.Angle(transform.forward, directionToTarget);
        if (angle <= 0.5f) return 100;

        float percentage = Mathf.Clamp01(1 - (angle / maxAngle));
        return percentage * 100;
    }

    public static float GetMaxAngle(Vector3 target, Transform transform)
    {
        Vector3 targetVect = new Vector3(target.x, 0, target.z);
        Vector3 vectr = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 directionToTarget = (targetVect - vectr).normalized;
        return Vector3.Angle(transform.forward, directionToTarget);
    }
}
