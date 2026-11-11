using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EntityActionSkill
{
    void BeginFacing(Entity entity, MagicSkillUseDto magic, Entity target, bool selfCast)
    {
        if (entity.Identity == null)
            return;

        int objectId = entity.Identity.Id;
        if (selfCast || target == null)
        {
            CombatPacketServices.Facing?.EndFollow(objectId, "skill-self-or-no-target");
            return;
        }

        bool follow = IsMagicSkill(magic) || CombatFacingService.IsUsingBow(entity);
        if (follow)
        {
            int rate = IsMagicSkill(magic)
                ? L2PawnYaw.MagicRotationRateYaw
                : L2PawnYaw.RotationRateYaw;
            CombatPacketServices.EnsureFacing().BeginFollow(objectId, entity.transform, target.transform, rate);
            return;
        }

        CombatPacketServices.Facing?.EndFollow(objectId, "non-bow-phys");
        _pawnYaw.RequestLook(entity.transform, target.transform.position, L2PawnYaw.RotationRateYaw);
    }
}
