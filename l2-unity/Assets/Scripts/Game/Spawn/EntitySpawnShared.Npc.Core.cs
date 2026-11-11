using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static partial class EntitySpawnShared
{
    public static void ApplyNpcIdentity(Entity npc, NpcSpawnRequest request)
    {
        EntityIdentity identity = request.Identity;
        Npcgrp npcgrp = request.Npcgrp;
        NpcName npcName = request.NpcName;

        Appearance appearance = new Appearance();
        appearance.RHand = npcgrp.Rhand;
        appearance.LHand = npcgrp.Lhand;
        appearance.CollisionRadius = npcgrp.CollisionRadius;
        appearance.CollisionHeight = npcgrp.CollisionHeight;

        string packetName = identity.Name;
        string packetTitle = identity.Title;
        npc.Status = request.Status;
        npc.Stats = request.Stats;
        npc.Identity = identity;
        npc.Identity.NpcClass = npcgrp.ClassName;
        bool pet = PetVisuals.IsPet(identity.Owned, identity.NpcId);
        if (!pet)
        {
            npc.Identity.Name = npcName.Name;
            npc.Identity.Title = npcName.Title;
        }
        else
        {
            npc.Identity.Name = string.IsNullOrEmpty(packetName) ? npcName.Name : packetName;
            npc.Identity.Title = string.IsNullOrEmpty(packetTitle) ? npcName.Title : packetTitle;
        }
        if ((npc.Identity.Title == null || npc.Identity.Title.Length == 0) &&
            identity.EntityType == EntityType.Monster)
        {
            npc.Identity.Title = " Lvl: " + npc.Stats.Level;
        }

        npc.Identity.TitleColor = npcName.TitleColor;
        npc.Appearance = appearance;
        npc.SetDead(false);
        SeedNpcHpFromGrp(npc, npcgrp);
    }

    public static void ResetNpcRuntimeForSpawn(Entity npc)
    {
        if (npc == null)
            return;

        npc.Running = npc.Identity != null && npc.Identity.IsRunning;
        npc.InCombat = false;
        npc.IsSoulshotCharged = false;
        npc.IsSpiritshotCharged = false;
        npc.TargetId = 0;
        npc.Target = null;
        npc.LastTarget = null;
        npc.AttackTarget = null;
        npc.ActionSlot.Clear();
    }

    public static void InitializeNpcAnimation(NetworkAnimationController controller)
    {
        if (controller == null)
            return;

        Animator animator = controller.GetAnimator();
        if (animator != null)
        {
            animator.enabled = true;
            animator.speed = 1f;
        }

        controller.Initialize();
        controller.SetAnimatorSpeed(1f);
        controller.SetPAtkSpeed(1f);

        animator = controller.GetAnimator();
        if (animator == null)
            return;

        int waitHash = Animator.StringToHash("wait");
        if (animator.HasState(0, waitHash))
        {
            animator.Play(waitHash, 0, 0f);
            animator.Update(0f);
        }

        CrowdAnimTest.Apply(controller);
    }

    public static void LogStriderAppear(string when, Entity entity, string detail = null)
    {
        if (entity == null || entity.Identity == null)
            return;
        int npcId = entity.Identity.NpcId;
        if (npcId != 12526 && npcId != 12527 && npcId != 12528)
            return;

        Animator animator = entity.GetComponentInChildren<Animator>(true);
        int waitHash = Animator.StringToHash("wait");
        bool hasWait = animator != null && animator.HasState(0, waitHash);
        bool isWait = animator != null && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == waitHash;
        string clip = "(none)";
        string controller = "(none)";
        string speed = "-";
        string enabled = "-";
        if (animator != null)
        {
            controller = animator.runtimeAnimatorController != null
                ? animator.runtimeAnimatorController.name
                : "(no controller)";
            speed = animator.speed.ToString("F3");
            enabled = animator.enabled.ToString() + " patkspd=" + animator.GetFloat("patkspd").ToString("F3");
            AnimatorClipInfo[] infos = animator.GetCurrentAnimatorClipInfo(0);
            if (infos.Length > 0 && infos[0].clip != null)
                clip = infos[0].clip.name + " w=" + infos[0].weight.ToString("F2");
        }

        Debug.Log(
            "[STRIDER_WAIT] when=" + when +
            " npc=" + npcId +
            " controller=" + controller +
            " enabled=" + enabled +
            " speed=" + speed +
            " hasWait=" + hasWait +
            " stateIsWait=" + isWait +
            " clip=" + clip +
            (string.IsNullOrEmpty(detail) ? "" : " " + detail));
    }

    public static bool IsStriderNpc(Entity entity)
    {
        if (entity == null || entity.Identity == null)
            return false;
        int npcId = entity.Identity.NpcId;
        return npcId == 12526 || npcId == 12527 || npcId == 12528;
    }
}
