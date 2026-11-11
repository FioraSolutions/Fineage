using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using VContainer;

public sealed partial class EntityActionMachine : ITickable
{
    [System.Obsolete("Use EntityActionPacketServices.Actions or [Inject] EntityActionMachine instead")]
    public static EntityActionMachine Instance { get; private set; }

    private readonly Dictionary<EntityActionKind, IEntityActionProcess> _processes;
    private readonly L2PawnRange _pawnRange;
    private readonly List<Entity> _active = new List<Entity>();
    private readonly HashSet<int> _activeIds = new HashSet<int>();

    public EntityActionMachine(
        L2PawnRange pawnRange,
        L2PawnYaw pawnYaw,
        IAnimationManager animations,
        EffectManager effects,
        MoveAllCharacters moves)
    {
        Instance = this;
        EntityActionPacketServices.Bind(this);
        ControllerPacketServices.Bind(
            animations: animations,
            effects: effects,
            moves: moves);
        AnimationPacketServices.Bind(animations: animations, moves: moves);
        MoveAllCharactersPacketServices.Bind(moves: moves);
        _pawnRange = pawnRange;
        _processes = new Dictionary<EntityActionKind, IEntityActionProcess>
        {
            { EntityActionKind.Idle, new EntityActionIdle() },
            { EntityActionKind.Move, new EntityActionMove(pawnRange, pawnYaw) },
            { EntityActionKind.Stop, new EntityActionStop(pawnRange) },
            { EntityActionKind.Attack, new EntityActionAttack(pawnYaw) },
            { EntityActionKind.Skill, new EntityActionSkill(pawnYaw) }
        };
    }

    public static bool IsAllowed(Entity entity)
    {
        return entity != null && !(entity is PlayerEntity);
    }

    public static bool ShouldHoldCombatIdle(Entity entity)
    {
        if (entity == null || !entity.InCombat)
            return false;
        if (entity.ActionSlot.Action == EntityActionKind.Move)
            return false;
        return entity.ActionSlot.Target == null && entity.AttackTarget == null;
    }

    public void Set(Entity entity, EntityActionKind action, object payload)
    {
        MagicSkillUseDto windMagic = payload as MagicSkillUseDto;
        bool windstrike = windMagic != null && windMagic.SkillId == 4247 &&
            action == EntityActionKind.Skill;
        if (!IsAllowed(entity))
        {
            if (windstrike)
            {
                Debug.Log(
                    "[WINDSTRIKE] Machine.Set blocked type=" +
                    (entity != null ? entity.GetType().Name : "null"));
            }
            return;
        }

        EntityActionSlot slot = entity.ActionSlot;
        if (windstrike)
        {
            Debug.Log(
                "[WINDSTRIKE] Machine.Set from=" + slot.Action +
                " to=Skill");
        }
        // if (NpcCombatTrace.IsCombatNpc(entity))
            // NpcCombatTrace.Write("ACTION", entity, null,
                // "from=" + slot.Action + " to=" + action + " " + NpcCombatTrace.Place(entity));
        // EntityActionCombatLog.RememberBeforeSet(slot.Action);
        slot.Write(action, payload, slot.Target, slot.Destination);

        IEntityActionProcess process;
        if (_processes.TryGetValue(action, out process))
        {
            if (windstrike)
                Debug.Log("[WINDSTRIKE] Machine.Enter " + process.GetType().Name);
            process.Enter(entity, payload);
            if (windstrike)
                Debug.Log("[WINDSTRIKE] Machine.Enter returned");
        }
        else if (windstrike)
        {
            Debug.Log("[WINDSTRIKE] Machine.Set no process for " + action);
        }

        Track(entity, action);
    }

    public static bool IsFinishingSwing(Entity entity)
    {
        UserEntity user = entity as UserEntity;
        return user != null && user.IsAttackVisualPlaying();
    }

    public static bool IsSkillOrAttack(EntityActionKind action)
    {
        return action == EntityActionKind.Attack || action == EntityActionKind.Skill;
    }

    void Track(Entity entity, EntityActionKind action)
    {
        if (entity.Identity == null)
            return;

        int id = entity.Identity.Id;
        if (action == EntityActionKind.Idle || action == EntityActionKind.Stop)
        {
            if (_activeIds.Remove(id))
                _active.Remove(entity);
            return;
        }

        if (_activeIds.Add(id))
            _active.Add(entity);
    }
}

