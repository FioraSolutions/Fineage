using UnityEngine;

/// <summary>
/// World/Entity-tied vector helpers. Cycle-safe conversions live on <see cref="VectorUtils"/>.
/// </summary>
public static class VectorWorldUtils
{
    static float _verticalVelocity;
    const float Gravity = 28f;

    public static Vector3 ApplyGravityGround(Vector3 position, float elapsedTime)
    {
        RaycastHit hit = default;
        World world = WorldApplyPacketServices.GameWorld ?? GamePacketServices.GameWorld;
        int groundMask = world != null ? world.GroundMask : 0;
        bool isGrounded = groundMask != 0 &&
            Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, groundMask);

        if (isGrounded)
        {
            position.y = hit.point.y;
            _verticalVelocity = 0;
        }
        else
        {
            _verticalVelocity -= Gravity * elapsedTime;
            position.y += _verticalVelocity * elapsedTime;
        }

        if (isGrounded && position.y < hit.point.y)
        {
            position.y = hit.point.y;
            _verticalVelocity = 0;
        }

        return position;
    }

    public static Vector3 GetCollision(Vector3 attacker, Transform target)
    {
        // UserInfo stores raw L2 half-height (~24). npcgrp is already /52.5.
        // 24 * 1.25 was aiming the bolt ~30m above the player.
        float particleHeight = AimHeightMeters(target);

        float distanceToAttacker = Vector3.Distance(attacker, target.position);

        if (distanceToAttacker < 5.0f)
        {
            return target.position + Vector3.up * particleHeight;
        }

        var heading = attacker - target.position;
        float angle = Vector3.Angle(heading, target.forward);
        Vector3 direction = Quaternion.Euler(0, angle, 0) * target.forward;
        return target.position + direction * 0.15f + Vector3.up * particleHeight;
    }

    static float AimHeightMeters(Transform target)
    {
        Entity entity = target != null ? target.GetComponent<Entity>() : null;
        float raw = entity != null && entity.Appearance != null
            ? entity.Appearance.CollisionHeight
            : 0f;
        return L2NameplateAnchor.CollisionHeightToUnityMeters(raw) * 1.25f;
    }
}
