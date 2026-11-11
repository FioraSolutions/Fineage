using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class WorldPacketApply
{
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
}
