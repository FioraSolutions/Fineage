using UnityEngine;

public static partial class VectorUtils
{
    public static Vector3 GetCollisionEffect(Transform attacker, Transform target, float height, float raduis)
    {
        var heading = target.position - attacker.position;
        float angle = Vector3.Angle(heading, attacker.forward);
        float particleHeight = height;
        Vector3 direction = Quaternion.Euler(0, angle, 0) * attacker.forward;
        return attacker.position + direction * raduis + Vector3.up * particleHeight;
    }

    public static Vector3 ConvertToUnityUnscaled(Vector3 ueVector)
    {
        return new Vector3(ueVector.y, ueVector.z, ueVector.x);
    }

    public static Vector3 ScaleToUnity(Vector3 ueVector)
    {
        return ueVector * L2UuToUnity;
    }

    public static float ConvertL2UuToMeters(float uu)
    {
        return uu * L2UuToUnity;
    }

    public static Vector2 RotateVector2(Vector2 vector, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        return RotateVector2Rad(vector, radians);
    }

    public static Vector2 RotateVector2Rad(Vector2 vector, float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        float newX = vector.x * cos - vector.y * sin;
        float newY = vector.x * sin + vector.y * cos;
        return new Vector2(newX, newY);
    }

    public static Vector3 FloorToNearest(Vector3 vector, float step)
    {
        return new Vector3(
            NumberUtils.FloorToNearest(vector.x, step),
            NumberUtils.FloorToNearest(vector.y, step),
            NumberUtils.FloorToNearest(vector.z, step));
    }

    public static Vector3 To2D(Vector3 pos)
    {
        return new Vector3(pos.x, 0, pos.z);
    }

    public static float Distance2D(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(To2D(from), To2D(to));
    }

    public static bool IsVectorZero2D(Vector3 vector)
    {
        return vector.x == 0 && vector.z == 0;
    }

    public static float CalculateMoveDirectionAngle(Vector3 from, Vector3 to)
    {
        float directionX = to.x - from.x;
        float directionZ = to.z - from.z;
        return CalculateMoveDirectionAngle(directionX, directionZ);
    }

    public static float CalculateMoveDirectionAngle(float directionX, float directionZ)
    {
        return Mathf.Atan2(directionX, directionZ) * Mathf.Rad2Deg;
    }

    public static float HeadingToUnityQuaternionForNpc(float heading)
    {
        return (heading / 182.04444444444444444444444444444f) % 360;
    }

    public static Vector3 CalcHitDirection(Vector3 currentPosition, Vector3 startPosition)
    {
        return (currentPosition - startPosition).normalized;
    }
}
