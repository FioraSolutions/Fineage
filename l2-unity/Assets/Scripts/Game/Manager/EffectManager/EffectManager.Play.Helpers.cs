using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public partial class EffectManager
{
    public void PlayEffectSyncedToSkillAnimation(
        int effectId,
        Entity entity,
        int hitTimeMs,
        AnimationCombo animCombo)
    {
        if (entity == null)
        {
            // Debug.LogWarning($"[SKILL_ANIM_FX] PlayEffectSyncedToSkillAnimation entity is null effectId={effectId}");
            return;
        }

        Transform weapon = entity.GetWeaponTransform();

        Transform attach = weapon != null ? weapon : entity.transform;
        IAnimationManager animations = _animations != null ? _animations : GamePacketServices.Animations;
        MagicCastData castData = SkillAnimationCastDataBuilder.Build(entity, hitTimeMs, animCombo, animations);
        PlayEffect(effectId, attach, castData);
    }

    void InjectSpawned(BaseEffect instance)
    {
        if (instance == null || _container == null)
        {
            return;
        }

        _container.InjectGameObject(instance.gameObject);
    }
}
