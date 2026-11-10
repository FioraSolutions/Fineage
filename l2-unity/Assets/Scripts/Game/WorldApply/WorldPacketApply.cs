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

}
