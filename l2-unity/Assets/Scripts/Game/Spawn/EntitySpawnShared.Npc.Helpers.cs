using UnityEngine;

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
            Gear gear = go.GetComponent<Gear>();
            if (gear != null)
                gear.Initialize(entity.Identity.Id, entity.RaceId);
        }

        entity.Initialize();
    }

    public static void RegisterAnimation(IWorldSpawnContext world, int objectId, IAnimationController controller, Entity entity)
    {
        if (world != null && world.Animations != null && controller != null)
            world.Animations.RegisterController(objectId, controller, entity);
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
