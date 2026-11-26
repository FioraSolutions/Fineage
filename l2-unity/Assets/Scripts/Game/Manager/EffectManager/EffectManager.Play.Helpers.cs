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
        AnimationCombo animCombo,
        int targetObjectId = 0)
    {
        if (entity == null)
        {
            // Debug.LogWarning($"[SKILL_ANIM_FX] PlayEffectSyncedToSkillAnimation entity is null effectId={effectId}");
            return;
        }

        // Skill composites pick sockets via <see cref="DefaultEffectAttachmentResolver"/>.
        // Using the weapon transform here made ground/root (_ca) FX ride the hand or blade
        // (Detect Weakness / howl at head height). Anchor resolve context on the pawn root.
        Transform attach = entity.transform;
        IAnimationManager animations = _animations != null ? _animations : GamePacketServices.Animations;
        MagicCastData castData = SkillAnimationCastDataBuilder.Build(entity, hitTimeMs, animCombo, animations, targetObjectId);
        PlayEffect(effectId, attach, castData);
    }

    /// <summary>
    /// Plays a composite built by the skill-visual resolver for this cast.
    /// </summary>
    public void PlayResolvedVisual(BaseEffect instance, EffectSettings settings, MagicCastData castData, Transform owner)
    {
        if (instance == null || owner == null)
        {
            Debug.LogError(
                "[SkillVisual] PlayResolvedVisual abort instance=" + (instance != null) +
                " owner=" + (owner != null));
            return;
        }

        if (!TryGateSkillFx(owner, castData))
        {
            Destroy(instance.gameObject);
            return;
        }

        InjectSpawned(instance);
        instance.gameObject.SetActive(true);
        instance.Setup(settings, castData, owner);
        instance.Play();
        TrackFxBudget(instance.gameObject, owner, castData);
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
