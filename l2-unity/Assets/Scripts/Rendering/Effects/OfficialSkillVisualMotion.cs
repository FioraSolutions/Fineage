using UnityEngine;

/// <summary>
/// Applies official LineageEffect RotPerSecond to a spawned skill-visual part.
/// Unreal rotator units per second are converted with VectorUtils.ConvertRotToUnity.
/// Launched projectiles are owned by ProjectileManager (same rate on ProjectileData).
/// </summary>
public sealed class OfficialSkillVisualMotion : MonoBehaviour
{
    public Vector3 RotPerSecondEuler;

    public static void Attach(BaseEffect instance, CompositePart part)
    {
        if (instance == null || part == null)
            return;
        if (part.IsLaunchedProjectile)
            return;
        if (part.rotPerSecondEuler == Vector3.zero)
            return;

        OfficialSkillVisualMotion motion = instance.GetComponent<OfficialSkillVisualMotion>();
        if (motion == null)
            motion = instance.gameObject.AddComponent<OfficialSkillVisualMotion>();
        motion.RotPerSecondEuler = part.rotPerSecondEuler;
    }

    void LateUpdate()
    {
        if (RotPerSecondEuler == Vector3.zero)
            return;
        transform.Rotate(RotPerSecondEuler * Time.deltaTime, Space.Self);
    }
}
