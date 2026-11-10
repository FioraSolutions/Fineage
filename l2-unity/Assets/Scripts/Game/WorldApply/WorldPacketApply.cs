using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class WorldPacketApply
{
    private readonly Dictionary<Type, EntityWorldApply> _byType = new Dictionary<Type, EntityWorldApply>();

    /// <summary>
    /// Per-object spawn attempt counter. A prefab that spawns without its NpcEntity, or a spawner
    /// that never gets wired, would otherwise be retried every frame forever.
    /// </summary>
    private readonly Dictionary<int, int> _spawnAttempts = new Dictionary<int, int>();
    private const int MaxSpawnAttempts = 3;
    private readonly HtmlWindow _html;
    private readonly PlayerPositionSender _positionSender;
    private readonly GameManager _gameManager;
    private readonly PacketApplyQueue _applyQueue;
    private readonly IGameEventBus _gameEvents;

    public WorldPacketApply(
        HtmlWindow html,
        PlayerPositionSender positionSender,
        PlayerWorldApply playerApply,
        NpcWorldApply npcApply,
        MonsterWorldApply monsterApply,
        UserWorldApply userApply,
        GameManager gameManager,
        PacketApplyQueue applyQueue,
        IGameEventBus gameEvents)
    {
        _html = html;
        _positionSender = positionSender;
        _gameManager = gameManager;
        _applyQueue = applyQueue;
        _gameEvents = gameEvents;
        Register<PlayerEntity>(playerApply);
        Register<NpcEntity>(npcApply);
        Register<MonsterEntity>(monsterApply);
        Register<UserEntity>(userApply);
    }

    private World GameWorld => WorldApplyPacketServices.GameWorld;

    public void Register<TEntity>(EntityWorldApply apply) where TEntity : Entity
    {
        _byType[typeof(TEntity)] = apply;
    }

    public void UpdateNpc(NpcInfoDto npcInfo)
    {
        if (GameWorld == null || npcInfo == null || npcInfo.Identity == null)
            return;
        if (!IsWorldSpawnReady())
            return;

        int objectId = npcInfo.Identity.Id;
        Entity entity = GameWorld.GetEntityNoLockSync(objectId);
        if (entity == null)
        {
            if (SpawnFrameBudget.IsCancelled(objectId))
                return;
            if (!SpawnFrameBudget.TryConsumeSpawn())
            {
                QueueWorldNextFrame(apply => apply.UpdateNpc(npcInfo));
                return;
            }
            GameWorld.SpawnNpc(npcInfo.Identity, npcInfo.Status, npcInfo.Stats, out bool unresolvable);

            // SpawnNpc bails silently when the npcgrp mesh cannot be resolved. Re-queueing such an
            // entry re-runs the same doomed attempt forever: the deferred action is put back on the
            // next batch, the batch is drained to the end every frame, so thousands of stale rows
            // spun at full CPU and the queue never drained (deferred grew past 35k). Drop the entry
            // from the knownlist instead so it stops being retried.
            if (unresolvable)
            {
                StorageNpc.getInstance().RemoveNpc(objectId);
                _spawnAttempts.Remove(objectId);
                return;
            }

            if (GameWorld.GetEntityNoLockSync(objectId) == null)
            {
                // Spawner not wired yet, or the prefab lacked its NpcEntity. Retrying is right, but
                // bounded: an unbounded retry is what turned a bad prefab into a permanent stall.
                int attempts;
                _spawnAttempts.TryGetValue(objectId, out attempts);
                attempts++;
                if (attempts > MaxSpawnAttempts)
                {
                    _spawnAttempts.Remove(objectId);
                    StorageNpc.getInstance().RemoveNpc(objectId);
                    return;
                }

                _spawnAttempts[objectId] = attempts;
                QueueWorldNextFrame(apply => apply.UpdateNpc(npcInfo));
                return;
            }

            _spawnAttempts.Remove(objectId);
            _gameEvents?.Publish(new EntitySpawnedEvent(objectId));
        }
        else
            GameWorld.UpdateNpc(entity, npcInfo);
    }

    public void FlushKnownlist()
    {
        StorageNpc storage = StorageNpc.getInstance();
        NpcInfoDto[] npcs = storage.CopyNpcs();
        CharInfoDto[] chars = storage.CopyChars();
        // Debug.Log("[WorldSpawn] FlushKnownlist npcs=" + npcs.Length + " chars=" + chars.Length);

        // Spawning every known object in one go defeats SpawnFrameBudget: each NPC over the cap
        // re-queues itself as its own deferred action, so a stale knownlist turns one flush into
        // tens of thousands of queue entries that all replay on the following frame. Apply what the
        // budget allows and hand the remainder to the next frame as a single continuation.
        int npcIndex = 0;
        for (; npcIndex < npcs.Length; npcIndex++)
        {
            if (!SpawnFrameBudget.HasBudget())
                break;
            UpdateNpc(npcs[npcIndex]);
        }

        int charIndex = 0;
        for (; charIndex < chars.Length; charIndex++)
            UpdateUser(chars[charIndex]);

        if (npcIndex < npcs.Length)
        {
            NpcInfoDto[] remaining = new NpcInfoDto[npcs.Length - npcIndex];
            System.Array.Copy(npcs, npcIndex, remaining, 0, remaining.Length);
            QueueWorldNextFrame(apply => apply.UpdateNpcBatch(remaining));
        }
    }

    /// <summary>
    /// Applies a chunk of knownlist rows, re-queueing itself while the spawn budget is exhausted.
    /// One continuation per frame instead of one queued action per NPC.
    /// </summary>
    public void UpdateNpcBatch(NpcInfoDto[] batch)
    {
        if (batch == null || batch.Length == 0)
            return;

        int index = 0;
        for (; index < batch.Length; index++)
        {
            if (!SpawnFrameBudget.HasBudget())
                break;
            UpdateNpc(batch[index]);
        }

        if (index < batch.Length)
        {
            NpcInfoDto[] remaining = new NpcInfoDto[batch.Length - index];
            System.Array.Copy(batch, index, remaining, 0, remaining.Length);
            QueueWorldNextFrame(apply => apply.UpdateNpcBatch(remaining));
        }
    }

    public void UpdateUser(CharInfoDto info)
    {
        if (GameWorld == null || info == null || info.Identity == null)
        {
            // GearFlowLog.Warn("UpdateUser abort world/info null");
            return;
        }
        if (!IsWorldSpawnReady())
        {
            // GearFlowLog.Warn("UpdateUser abort world not ready id=" + info.Identity.Id +
                // " nick=" + info.Identity.Name + " " + GearFlowLog.Paperdoll(info.Appearance));
            return;
        }

        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local != null && local.Identity != null && local.Identity.Id == info.Identity.Id)
        {
            // GearFlowLog.Info("UpdateUser SKIP local id=" + info.Identity.Id);
            return;
        }

        Entity entity = GameWorld.GetEntityNoLockSync(info.Identity.Id);
        if (entity == null)
        {
            if (SpawnFrameBudget.IsCancelled(info.Identity.Id))
                return;
            if (!SpawnFrameBudget.TryConsumeSpawn())
            {
                QueueWorldNextFrame(apply => apply.UpdateUser(info));
                return;
            }
            GameWorld.SpawnUser(info);
            _gameEvents?.Publish(new EntitySpawnedEvent(info.Identity.Id));
            // PartySpelled may have arrived before CharInfo finished spawning the peer.
            AbnormalStatusUpdateApplyIncomingPacketServices.FlushPendingPeerAbnormals(info.Identity.Id);
            return;
        }

        if (entity is UserEntity)
        {
            // GearFlowLog.Info("UpdateUser APPLY UserEntity id=" + info.Identity.Id +
                // " nick=" + info.Identity.Name +
                // " type=" + entity.GetType().Name +
                // " have " + GearFlowLog.Paperdoll(entity) +
                // " want " + GearFlowLog.Paperdoll(info.Appearance));
            GameWorld.UpdateUser(entity, info);
            return;
        }

        // GearFlowLog.Warn("UpdateUser SKIP not UserEntity id=" + info.Identity.Id +
            // " type=" + entity.GetType().Name);
    }

    public void ShowNpcHtml(NpcHtmlMessageDto dto)
    {
        if (dto.GetNpcId() == 0 && _html != null)
        {
            _html.InjectToWindow(dto.Html);
            _html.ShowWindowToCenterAndBringToFront();
        }

        Entity npc = GetEntity(dto.GetNpcId());
        EntityWorldApply apply;
        if (npc == null || !TryResolve(npc, out apply))
            return;

        apply.OnNpcHtml(npc, dto);
    }

    public void SendArrivedPosition()
    {
        PlayerController player = WorldApplyPacketServices.Player;
        if (player == null)
            return;

        _positionSender.SendServerArrivedPosition(player.transform.position);
    }

    private Entity GetEntity(int objId)
    {
        return GameWorld == null ? null : GameWorld.GetEntityNoLockSync(objId);
    }

    private bool TryResolve(Entity entity, out EntityWorldApply apply)
    {
        Type type = entity.GetType();
        while (type != null && type != typeof(Entity) && type != typeof(object))
        {
            if (_byType.TryGetValue(type, out apply))
                return true;
            type = type.BaseType;
        }

        apply = null;
        return false;
    }

    private bool IsWorldSpawnReady()
    {
        GameManager manager = _gameManager != null ? _gameManager : WorldApplyPacketServices.Manager;
        return manager != null && manager.WorldSpawnReady && GameWorld != null;
    }

    private void QueueWorldNextFrame(System.Action<WorldPacketApply> apply)
    {
        if (apply == null)
            return;
        if (_applyQueue != null)
            _applyQueue.QueueNextFrame(() => apply(this));
        else
            WorldApplyPacketServices.QueueWorldNextFrame(apply);
    }
}
