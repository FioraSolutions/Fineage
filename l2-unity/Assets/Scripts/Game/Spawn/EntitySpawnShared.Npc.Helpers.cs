using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class EntitySpawnShared
{
    public static void LogStriderGait(string when, Entity entity, string detail = null)
    {
        if (!IsStriderNpc(entity))
            return;

        Animator animator = entity.GetComponentInChildren<Animator>(true);
        string state = "-";
        string clip = "(none)";
        if (animator != null)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("run"))
                state = "run";
            else if (info.IsName("walk"))
                state = "walk";
            else if (info.IsName("wait"))
                state = "wait";
            else if (info.IsName("atk01"))
                state = "atk01";
            else if (info.IsName("atkwait"))
                state = "atkwait";
            else
                state = "other";
            AnimatorClipInfo[] infos = animator.GetCurrentAnimatorClipInfo(0);
            if (infos.Length > 0 && infos[0].clip != null)
                clip = infos[0].clip.name;
        }

        IMoveAllCharacters moves = MoveAllCharactersPacketServices.Moves;
        bool moving = moves != null && moves.IsMoving(entity.Identity.Id);
        string action = entity.ActionSlot != null ? entity.ActionSlot.Action.ToString() : "-";
        Debug.Log(
            "[STRIDER_GAIT] when=" + when +
            " npc=" + entity.Identity.NpcId +
            " id=" + entity.Identity.Id +
            " active=" + entity.gameObject.activeInHierarchy +
            " action=" + action +
            " moving=" + moving +
            " running=" + entity.Running +
            " inCombat=" + entity.InCombat +
            " state=" + state +
            " clip=" + clip +
            (string.IsNullOrEmpty(detail) ? "" : " " + detail));
    }

    static void SeedNpcHpFromGrp(Entity npc, Npcgrp npcgrp)
    {
        if (npc == null || npcgrp == null || npc.Stats == null || npc.Status == null)
            return;

        // Zone EntitySpawn already seeded MaxHp/CurHp (#522). Overwriting with npcgrp made
        // select-target StatusUpdate look like damage and play hit SFX.
        if (npc.Stats.MaxHp > 1)
            return;

        int grpHp = Mathf.Max(1, Mathf.RoundToInt(npcgrp.MaxHp));
        if (grpHp <= 1)
            return;

        npc.Stats.MaxHp = grpHp;
        npc.Status.SetHp(grpHp);
    }

    public static void BindGearAndAnimation(Entity entity, GameObject go, IAnimationController controller)
    {
        if (go != null)
        {
            EnsureNpcVisualPolicy(go, entity);
            Gear gear = EnsureNpcGear(go, entity);
            if (gear != null)
                gear.Initialize(entity.Identity.Id, entity.RaceId);
        }

        entity.Initialize();
    }

    static Gear EnsureNpcGear(GameObject go, Entity entity)
    {
        if (go == null || entity == null || entity.Appearance == null)
            return go != null ? go.GetComponent<Gear>() : null;

        bool needsWeapons = entity.Appearance.RHand > 0 || entity.Appearance.LHand > 0;
        Gear gear = go.GetComponent<Gear>();
        if (gear == null && needsWeapons)
        {
            gear = go.AddComponent<Gear>();
            Debug.Log(
                "[NpcGear] added Gear for npc id="
                + entity.Identity.NpcId
                + " mesh="
                + go.name
                + " rhand="
                + entity.Appearance.RHand
                + " lhand="
                + entity.Appearance.LHand);
        }

        return gear;
    }

    static void EnsureNpcVisualPolicy(GameObject go, Entity entity)
    {
        if (go == null)
            return;

        L2ActorMeshPolicy.DisableSkeletonPlaceholders(go.transform);
        L2ActorMeshPolicy.DisableAlphaOverlays(go.transform);

        int npcId = entity != null && entity.Identity != null ? entity.Identity.NpcId : 0;
        float bodyHeight = entity != null && entity.Appearance != null
            ? Mathf.Max(0.5f, entity.Appearance.CollisionHeight / 52.5f)
            : 1.8f;
        // Guards / pole weapons inflate SMR bounds without being a bad import (npcId 31033 etc.).
        float maxHeight = bodyHeight * 2.75f + 0.4f;
        SkinnedMeshRenderer[] skins = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < skins.Length; i++)
        {
            SkinnedMeshRenderer smr = skins[i];
            if (smr == null || !smr.enabled)
                continue;
            float h = smr.bounds.size.y;
            if (L2ActorMeshPolicy.IsSkeletonPlaceholder(smr.name)
                || L2ActorMeshPolicy.IsAlphaOverlayRenderer(smr))
            {
                smr.enabled = false;
                continue;
            }

            if (h > maxHeight)
            {
                bool severe = h > maxHeight * 1.45f;
                string msg =
                    "[NpcMesh] distorted skinned bounds mesh="
                    + smr.name
                    + " npcId="
                    + npcId
                    + " height="
                    + h.ToString("F2")
                    + "m limit="
                    + maxHeight.ToString("F2")
                    + "m material="
                    + (smr.sharedMaterial != null ? smr.sharedMaterial.name : "(null)");
                if (severe)
                    Debug.LogError(msg);
                else
                    Debug.LogWarning(msg + " (may be weapon reach; not necessarily a bad mesh)");
            }

            Material mat = smr.sharedMaterial;
            if (mat != null && L2ActorMaterialSetup.MaterialLooksLikeActorOpaqueMismatch(mat))
                L2ActorMaterialSetup.LogOpaqueAlphaMismatch(mat, smr.name);
        }
    }

    public static void RegisterAnimation(IWorldSpawnContext world, int objectId, IAnimationController controller, Entity entity)
    {
        if (world != null && world.Animations != null && controller != null)
            world.Animations.RegisterController(objectId, controller, entity);
    }

    public static void ApplyNpcCollisionVisualScale(GameObject go, Npcgrp npcgrp)
    {
        if (go == null || npcgrp == null || npcgrp.CollisionHeight <= 0.01f)
            return;

        float targetHeight = npcgrp.CollisionHeight / 52.5f;
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                bounds.Encapsulate(renderers[i].bounds);
        }

        float meshHeight = bounds.size.y;
        if (meshHeight > 0.001f && meshHeight < targetHeight * 0.45f)
        {
            float scale = targetHeight / meshHeight;
            Transform root = go.transform;
            root.localScale = root.localScale * scale;
            Debug.Log(
                "[NpcScale] structure mesh="
                + go.name
                + " npcgrp="
                + npcgrp.ClassName
                + " meshHeight="
                + meshHeight.ToString("F3")
                + "m target="
                + targetHeight.ToString("F3")
                + "m scale="
                + scale.ToString("F3"));
        }

        CharacterController cc = go.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.height = Mathf.Max(targetHeight, cc.height);
            cc.radius = Mathf.Max(npcgrp.CollisionRadius / 52.5f, cc.radius);
            cc.center = new Vector3(0f, cc.height * 0.5f, 0f);
        }
    }

    public static void ApplyNpcMoveSpeeds(Entity npc, bool useRealPAtk)
    {
        if (useRealPAtk)
            npc.UpdateNpcPAtkSpd((int)npc.Stats.PAtkRealSpeed);
        else
            npc.UpdateNpcPAtkSpd((int)npc.Stats.PAtkSpd);
        npc.UpdateNpcRunningSpd(npc.Stats.RunRealSpeed);
        npc.UpdateNpcWalkSpd(npc.Stats.WalkRealSpeed);
        npc.Running = npc.Identity.IsRunning;
    }
}
