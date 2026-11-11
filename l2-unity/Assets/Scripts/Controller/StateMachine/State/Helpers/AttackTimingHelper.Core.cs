using UnityEngine;
using System;

public static partial class AttackTimingHelper
{
    public static void RotateFaceToMonster(Entity entity)
    {
        // Transform monster = DomainPacketServices.LocalPlayer.Target;
        // if (monster == null || entity == null) return;
        //
        // RotationService.Instance.RotateTowards(entity.transform, monster.position, () =>
        // {
        //     Entity monsterEntity = monster.GetComponent<Entity>();
        //     if (monsterEntity == null) return;
        //
        //     float monsterHeight = monsterEntity.Appearance.CollisionHeight;
        //     Vector3 monsterFacePosition = monster.position + Vector3.up * (monsterHeight * 0.8f);
        //
        //     Vector3 startPoint = entity.transform.position + Vector3.up * 1.5f;
        //     Vector3 lookDir = (monsterFacePosition - startPoint).normalized;
        //     float verticalAngle = Mathf.Asin(lookDir.y) * Mathf.Rad2Deg;
        //
        //     float spineAngle = Mathf.Clamp(verticalAngle * 0.4f, -15f, 10f);
        //     Vector3 spineRotation = new Vector3(0, 0, spineAngle);
        //
        //     float armAngle = Mathf.Clamp(verticalAngle * 0.3f, -20f, 10f);
        //     Vector3 armRotation = new Vector3(0, 0, armAngle);
        //
        //     if (entity is PlayerEntity playerEntity)
        //     {
        //         playerEntity.SetProceduralSpinePose(spineRotation);
        //         playerEntity.SetProceduralRightUpperArmPose(armRotation);
        //     }
        // });
    }

    public static float ResolveServerLikeAttackDurationMs(PlayerEntity player)
    {
        return ResolveAttackCycleMs(player, null);
    }

    public static float ResolvePAtkSpd(Entity entity)
    {
        if (entity == null || entity.Stats == null)
            return 333f;

        if (entity is PlayerEntity && entity.Stats.BasePAtkSpeed > 1f)
            return entity.Stats.BasePAtkSpeed;

        if (entity.Stats.PAtkSpd > 1)
            return entity.Stats.PAtkSpd;

        if (entity.Stats.BasePAtkSpeed > 1f)
            return entity.Stats.BasePAtkSpeed;

        return 333f;
    }

    public static float ResolveAttackCycleMs(Entity entity, string animName)
    {
        float pAtk = ResolvePAtkSpd(entity);
        float baseTimeAtkMs = CalcBaseParam.CalculateTimeL2j(pAtk);

        if (string.IsNullOrEmpty(animName) ||
            animName.IndexOf("bow", System.StringComparison.OrdinalIgnoreCase) < 0)
        {
            return baseTimeAtkMs;
        }

        float distance = 0f;
        if (entity is PlayerEntity player)
        {
            if (player.Target != null)
                distance = player.TargetDistance();
        }
        else if (entity != null && entity.Target != null)
        {
            distance = VectorUtils.Distance2D(entity.transform.position, entity.Target.position);
        }

        if (distance <= 0.01f)
            return baseTimeAtkMs;

        return CalcBaseParam.CalculateAttackAndFlightTimes(distance, baseTimeAtkMs)[0];
    }
}
