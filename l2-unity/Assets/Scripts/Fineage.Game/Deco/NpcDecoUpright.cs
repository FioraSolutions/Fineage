using UnityEngine;

/// <summary>
/// UC Z is world-up. Bone tilt would put PTDU_Normal feet discs edge-on or under the floor.
/// Shared by deco attachment + keep-up MonoBehaviour (#131).
/// </summary>
public static class NpcDecoUpright
{
    public static Quaternion UprightYaw(Transform source)
    {
        Vector3 forward = source != null ? source.forward : Vector3.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 1e-6f)
            forward = Vector3.forward;
        return Quaternion.LookRotation(forward.normalized, Vector3.up);
    }
}
