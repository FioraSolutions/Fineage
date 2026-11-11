using UnityEngine;

public partial class ProjectileData
{
    public ProjectileData(ProjectileData other)
    {
        if (other != null)
        {
            id = other.id;
            prefab = other.prefab;
            transform = other.transform;
            startPosition = other.startPosition;
            targetTransform = other.targetTransform;
            speed = other.speed;
            damage = other.damage;
            lifetime = other.lifetime;
            speedCurve = other.speedCurve != null ? other.speedCurve : CreateDefaultSpeedCurve();
            startTime = other.startTime;
            distance = other.distance;
            useGravity = other.useGravity;
            gravityScale = other.gravityScale;
            flytime = other.flytime;
            isActive = other.isActive;
            impactType = other.impactType;
            attackerEntityId = other.attackerEntityId;
        }

    }

    public static AnimationCurve CreateDefaultSpeedCurve()
    {
        return new AnimationCurve(
            new Keyframe(0, 0.3f, 0, 0.5f),
            new Keyframe(0.25f, 0.5f, 0, 1f),
            new Keyframe(0.5f, 1f, 0, 0),
            new Keyframe(0.75f, 1f, 0, 0),
            new Keyframe(1, 1f, 0, 0));
    }
}
