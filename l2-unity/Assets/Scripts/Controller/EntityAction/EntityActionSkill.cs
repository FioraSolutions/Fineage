using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EntityActionSkill : IEntityActionProcess
{
    readonly L2PawnYaw _pawnYaw;

    public EntityActionSkill(L2PawnYaw pawnYaw)
    {
        _pawnYaw = pawnYaw;
    }


    public void Tick(Entity entity)
    {
        if (entity == null || entity.IsDead())
            return;

        UserEntity user = entity as UserEntity;
        if (user != null && user.IsAttackVisualPlaying())
            return;

        Entity target = entity.ActionSlot.Target;
        if (target == null && entity.AttackTarget != null)
            target = entity.AttackTarget.GetComponent<Entity>();
        if (target != null && target != entity && target.IsDead())
        {
            MagicGaitTrace.Event(entity, "Skill.Tick-deadTarget→CancelAttackKeepStance", null);
            if (EntityActionPacketServices.Actions != null)
                EntityActionPacketServices.Actions.CancelAttackKeepStance(entity);
            return;
        }

        if (user == null || !user.ShouldReturnFromAttack())
            return;

        MagicGaitTrace.Event(entity, "Skill.Tick→Idle",
            "visExpired ClearAttackVisual → Set Idle");
        user.ClearAttackVisual();
        if (entity.Identity != null)
            CombatPacketServices.Facing?.EndFollow(entity.Identity.Id, "skill-tick");
        if (EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.Set(entity, EntityActionKind.Idle, null);
    }

    public static bool TryApplyWeaponCharge(Entity entity, MagicSkillUseDto magic)
    {
        if (entity == null || magic == null || !WeaponChargeSkill.IsCharge(magic.SkillId))
            return false;

        if (WeaponChargeSkill.IsSpirit(magic.SkillId))
        {
            entity.IsSpiritshotCharged = true;
            entity.IsSoulshotCharged = false;
        }
        else
        {
            entity.IsSoulshotCharged = true;
            entity.IsSpiritshotCharged = false;
        }

        WeaponChargeSkillPlayback.Play(
            ControllerPacketServices.Effects,
            magic.SkillId,
            entity.GetWeaponTransform(),
            entity.transform);

        return true;
    }

}
