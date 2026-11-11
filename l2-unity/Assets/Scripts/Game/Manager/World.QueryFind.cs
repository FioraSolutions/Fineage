using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using System.Security.Principal;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;

public partial class World
{
    public Task UpdateEntityTarget(int id, int targetId) {
        return ExecuteWithEntitiesAsync(id, targetId, (targeter, targeted) => {
            targeter.TargetId = targetId;
            targeter.Target = targeted.transform;
        });
    }


    public Task StatusUpdate(int id, List<StatusUpdate.Attribute> attributes) {
        return ExecuteWithEntityAsync(id, e => {
            if(_worldCombat != null)
            {
                _worldCombat.StatusUpdate(e, attributes, id);
                // UnityChainLog.OnStatusUpdate(id, attributes);
                // UnityReplaySession.OnStatus(e, attributes);
                if (e.GetType() == typeof(PlayerEntity))
                {
                    if (CharacterInfo != null)
                        CharacterInfo.UpdateValues();
                }
            }

        });
    }

    public Task UserInfoUpdateCharacter(UserInfoDto user)
    {
        return ExecuteWithEntityAsync(user.PlayerInfoInterlude.Identity.Id, e => {
            if (_worldCombat != null)
                _worldCombat.StatusUpdate(e, user.PlayerInfoInterlude.Stats, user.PlayerInfoInterlude.Status , user.PlayerInfoInterlude.Identity.Id);
            if (e == DomainPacketServices.LocalPlayer)
            {
                DomainPacketServices.LocalPlayer.Running = user.PlayerInfoInterlude.Appearance.Running;
                if (CharacterInfo != null)
                    CharacterInfo.UpdateValues();
            }
        });
    }

    public async Task Revive(int dieObj)
    {
        Entity entity = await GetEntityNoLock(dieObj);

        if (entity != null)
        {
            if (entity.GetType() == typeof(PlayerEntity))
            {

                PlayerStatePacketServices.PlayerState.ChangeState(PlayerState.REBIRTH);
                PlayerStatePacketServices.PlayerState.NotifyEvent(Event.REBIRTH);

                entity.SetDead(false);

            }
   
        }
    }

    public Task<Entity> GetEntityNoLock(int id)
    {
        return System.Threading.Tasks.Task.FromResult(GetEntityNoLockSync(id));
    }

    public Entity GetEntityNoLockSync(int id)
    {
        if (_objects.ContainsKey(id))
        {
            return _objects[id];
        }
        return null;
    }

    /// <inheritdoc cref="IWorldEntityLookup.FindTransformNoLock"/>
    public Transform FindTransformNoLock(int objectId)
    {
        Entity entity = GetEntityNoLockSync(objectId);
        return entity != null ? entity.transform : null;
    }

    public void ForEachEntity(Action<Entity> action)
    {
        if (action == null)
            return;

        foreach (KeyValuePair<int, Entity> pair in _objects)
        {
            if (pair.Value != null)
                action(pair.Value);
        }
    }

    /// <summary>
    /// Iterates remote players (UserEntity/CharInfo) + local PlayerEntity.
    /// These move via L2PawnWalk and have their CharacterController disabled, so
    /// physics queries (OverlapSphere/SphereCastAll) cannot discover them — use this
    /// list instead for nameplates and other per-entity bookkeeping.
    /// </summary>
    public void ForEachUser(Action<Entity> action)
    {
        if (action == null)
            return;

        foreach (KeyValuePair<int, Entity> pair in _players)
        {
            if (pair.Value != null)
                action(pair.Value);
        }
    }

    // Wait for entity to be fully loaded
    public async Task<Entity> GetEntityAsync(int id) {
        Entity entity;
        lock (_objects) {
            if (!_objects.TryGetValue(id, out entity)) {
                //Debug.LogWarning($"GetEntityAsync - Entity {id} not found, retrying...");
            }
        }

        if (entity == null) {
            await Task.Delay(150); // Wait for 150 ms retrying

            lock (_objects) {
                if (!_objects.TryGetValue(id, out entity)) {
                    // Debug.LogWarning($"GetEntityAsync - Entity {id} not found after retry");
                    return null;
                } else {
                   // Debug.LogWarning($"GetEntityAsync - Entity {id} found after retry");
                }
            }
        }

        return entity;
    }

    // Execute action after entity is loaded
    private async Task ExecuteWithEntityAsync(int id, Action<Entity> action) {
        var entity = await GetEntityAsync(id);
        if (entity != null) {
            try {
                _eventProcessor.QueueEvent(() => action(entity));
            } catch (Exception) {
                // Debug.LogWarning($"Operation failed - Target {id} - Error {ex.Message}");
            }
        }
    }

    // Execute action after 2 entities are loaded
    private async Task ExecuteWithEntitiesAsync(int id1, int id2, Action<Entity, Entity> action) {
        var entity1Task = GetEntityAsync(id1);
        var entity2Task = GetEntityAsync(id2);

        await Task.WhenAll(entity1Task, entity2Task);

        var entity1 = await entity1Task;
        var entity2 = await entity2Task;

        if (entity1 != null && entity2 != null) {
            try {
                _eventProcessor.QueueEvent(() => action(entity1, entity2));
            } catch (Exception) {
                // Debug.LogWarning($"Operation failed - Target {id1} or {id2} - Error {ex.Message}");
            }
        }
    }

    private bool IsWorldSpawnReady()
    {
        GameManager manager = _gameManager != null ? _gameManager : WorldPacketServices.Manager;
        WorldPacketApply apply = WorldPacketServices.WorldApply;
        return manager != null && manager.WorldSpawnReady && apply != null;
    }
}
