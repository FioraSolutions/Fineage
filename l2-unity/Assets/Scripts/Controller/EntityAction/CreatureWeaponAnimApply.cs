using UnityEngine;

/// <summary>
/// Town guards and monsters: weapon-suffixed locomotion clips from AnimTween via <see cref="NpcAnimationClipBinder"/>.
/// </summary>
public static class CreatureWeaponAnimApply
{
    public static void Bind(Entity entity)
    {
        if (entity == null || entity is UserEntity || entity is PlayerEntity)
            return;

        if (!EntityHasWeaponSlots(entity))
            return;

        NetworkAnimationController nac = entity.GetAnimatorController();
        NpcAnimationClipBinder.Bind(nac, entity);
    }

    static bool EntityHasWeaponSlots(Entity entity)
    {
        if (entity == null || entity.Appearance == null)
            return false;

        if (entity.Appearance.RHand > 0 || entity.Appearance.LHand > 0)
            return true;

        NpcEntity npc = entity as NpcEntity;
        Npcgrp grp = npc != null && npc.NpcData != null ? npc.NpcData.Npcgrp : null;
        return grp != null && (grp.Rhand > 0 || grp.Lhand > 0);
    }
}