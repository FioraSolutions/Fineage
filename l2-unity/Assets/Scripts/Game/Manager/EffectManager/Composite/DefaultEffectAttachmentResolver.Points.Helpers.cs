using System.Linq;
using UnityEngine;
using VContainer;

public partial class DefaultEffectAttachmentResolver
{
    private bool ResolveRoot(Transform src, out Transform resolvedTransform, out Vector3 worldPosition)
    {
        resolvedTransform = src;
        worldPosition = src != null ? src.position : Vector3.zero;
        return src != null;
    }

    /// <summary>
    /// CasterRoot must mean the actor's own root. Skill effects are started with the weapon as owner
    /// (<see cref="EffectManager.PlayEffectSyncedToSkillAnimation"/> passes entity.GetWeaponTransform()),
    /// so the raw CasterTransform is a sword/bow bone. Parenting a ground ring or a dash burst there
    /// made it ride the swing (and pick up the arm's sideways rotation) instead of staying under the feet.
    /// </summary>
    private bool ResolveCasterRoot(EffectResolveContext context, out Transform resolvedTransform, out Vector3 worldPosition)
    {
        resolvedTransform = null;
        worldPosition = Vector3.zero;

        Entity casterEntity = context != null ? context.CasterEntity : null;
        Transform entityRoot = casterEntity != null ? casterEntity.transform : null;
        Transform casterTransform = context != null ? context.CasterTransform : null;

        // Keep the entity root whenever the owner is a bone/weapon under it; fall back to the given
        // transform when the caster entity is unknown (deco, NPC rotating parts, detached FX).
        Transform root = entityRoot != null &&
                         casterTransform != null &&
                         casterTransform != entityRoot &&
                         casterTransform.IsChildOf(entityRoot)
            ? entityRoot
            : casterTransform ?? entityRoot;

        return ResolveRoot(root, out resolvedTransform, out worldPosition);
    }

    private bool ResolveLowerBody(Entity entity, Transform fallbackRoot, out Transform resolvedTransform, out Vector3 worldPosition)
    {
        resolvedTransform = null;
        worldPosition = Vector3.zero;

        // Prefer pelvis/hips for choosing follow transform when root is missing; world anchor matches follow pivot
        // when following entity root so Composite prefab positionOffset (tuned as root-local TransformPoint) stays valid.
        // For bone-centered hits without that offset, use TargetCenter / WorldHitPoint instead.
        Gear gear = entity != null ? entity.Gear : null;
        if (gear != null)
        {
            Transform pelvis = gear.FindRecursiveBone("Bip01_Pelvis") ?? gear.FindRecursiveBone("Bip01_Hips");
            if (pelvis != null)
            {
                resolvedTransform = fallbackRoot != null ? fallbackRoot : pelvis;
                worldPosition = resolvedTransform.position;
                return true;
            }

            Transform leftFoot = gear.FindRecursiveBone("Bip01_L_Foot");
            Transform rightFoot = gear.FindRecursiveBone("Bip01_R_Foot");
            if (leftFoot != null && rightFoot != null)
            {
                resolvedTransform = fallbackRoot;
                worldPosition = resolvedTransform != null
                    ? resolvedTransform.position
                    : (leftFoot.position + rightFoot.position) * 0.5f;
                return true;
            }
        }

        Animator animator = entity != null ? entity.Animator : null;
        if (animator != null && animator.isHuman && animator.avatar != null)
        {
            Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            if (hips != null)
            {
                resolvedTransform = fallbackRoot != null ? fallbackRoot : hips;
                worldPosition = resolvedTransform.position;
                return true;
            }

            Transform left = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            Transform right = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);

            if (left != null && right != null)
            {
                resolvedTransform = fallbackRoot;
                worldPosition = resolvedTransform != null
                    ? resolvedTransform.position
                    : (left.position + right.position) * 0.5f;
                return true;
            }
        }

        if (fallbackRoot != null)
        {
            resolvedTransform = fallbackRoot;
            worldPosition = fallbackRoot.position;
            return true;
        }

        return false;
    }

    private bool ResolveWeapon(
        Entity entity,
        Transform fallbackRoot,
        bool leftHand,
        out Transform resolvedTransform,
        out Vector3 worldPosition)
    {
        resolvedTransform = null;
        worldPosition = Vector3.zero;

        if (!leftHand && entity is PlayerEntity playerEntity)
        {
            Transform weapon = playerEntity.GetWeaponTransform();
            if (weapon != null)
            {
                resolvedTransform = weapon;
                worldPosition = weapon.position;
                return true;
            }
        }

        Gear gear = entity != null ? entity.Gear : null;
        if (gear != null)
        {
            Transform[] found = leftHand
                ? gear.GetAllTransformByLeftHand(WeaponNamePattern)
                : gear.GetAllTransformByRightHand(WeaponNamePattern);
            Transform guessed = found != null ? found.FirstOrDefault() : null;
            if (guessed != null)
            {
                resolvedTransform = guessed;
                worldPosition = guessed.position;
                return true;
            }
        }

        if (!leftHand && entity != null)
        {
            Transform guessed = entity.transform.Find("weapon_");
            if (guessed != null)
            {
                resolvedTransform = guessed;
                worldPosition = guessed.position;
                return true;
            }
        }

        // Empty hands and fists have no weapon_ mesh. weapon_shot.int still attaches to the hand.
        if (gear != null)
        {
            Transform bone = leftHand
                ? gear.GetTransformLeftHandBone()
                : gear.GetTransformRightHandBone();
            if (bone != null)
            {
                resolvedTransform = bone;
                worldPosition = bone.position;
                return true;
            }
        }

        if (fallbackRoot != null)
        {
            resolvedTransform = fallbackRoot;
            worldPosition = fallbackRoot.position;
            return true;
        }

        return false;
    }
}
