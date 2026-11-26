using UnityEngine;

/// <summary>Spawn-time checks for NPC base wait idle (Interlude: looping wait + rare SpWait). Item 13.</summary>
public static class NpcIdleAnimationBootstrap
{
    public static void Apply(NetworkAnimationController controller, Entity entity)
    {
        if (controller == null)
            return;

        Animator animator = controller.GetAnimator();
        if (animator == null)
            return;

        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        string mesh = ResolveMeshLabel(entity, animator);
        int npcId = entity != null && entity.Identity != null ? entity.Identity.NpcId : 0;

        NpcAnimationClipBinder.Bind(controller, entity);

        AnimationClip waitClip = NpcWaitClipResolver.ResolveBaseWait(animator);
        if (waitClip == null)
        {
            Debug.LogError(
                "[NpcIdle] missing base wait animation mesh="
                + mesh
                + " npcId="
                + npcId
                + " controller="
                + (animator.runtimeAnimatorController != null
                    ? animator.runtimeAnimatorController.name
                    : "(null)"));
            return;
        }

        if (!waitClip.isLooping
            && NpcWaitClipResolver.IsBaseLocomotionLoopName(waitClip.name))
        {
            Debug.LogWarning(
                "[NpcIdle] base wait clip is not looped in import; SMB will restart each cycle. mesh="
                + mesh
                + " anim="
                + waitClip.name
                + " npcId="
                + npcId);
        }

        NpcIdleScheduler.Ensure(entity);

        int waitHash = Animator.StringToHash("wait");
        if (animator.HasState(0, waitHash))
        {
            animator.Play(waitHash, 0, Random.value);
            animator.Update(0f);
        }
    }

    static string ResolveMeshLabel(Entity entity, Animator animator)
    {
        if (entity != null && entity.Identity != null)
        {
            Npcgrp grp = TablePacketServices.NpcGrps?.GetNpcgrp(entity.Identity.NpcId);
            if (grp != null && !string.IsNullOrEmpty(grp.Mesh))
                return grp.Mesh;
        }

        return animator != null ? animator.gameObject.name : "(unknown)";
    }
}