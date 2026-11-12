using System;
using System.Collections.Generic;
using System.Threading;
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
        return _entities.GetOrNull(id);
    }

    /// <inheritdoc cref="IWorldEntityLookup.FindTransformNoLock"/>
    public Transform FindTransformNoLock(int objectId)
    {
        Entity entity = GetEntityNoLockSync(objectId);
        return entity != null ? entity.transform : null;
    }

    public void ForEachEntity(Action<Entity> action)
    {
        _entities.ForEach(action);
    }

    /// <summary>
    /// Iterates remote players (UserEntity/CharInfo) + local PlayerEntity.
    /// These move via L2PawnWalk and have their CharacterController disabled, so
    /// physics queries (OverlapSphere/SphereCastAll) cannot discover them — use this
    /// list instead for nameplates and other per-entity bookkeeping.
    /// </summary>
    public void ForEachUser(Action<Entity> action)
    {
        _entities.ForEachPlayer(action);
    }

    /// <summary>
    /// Wait until <paramref name="id"/> is registered, or until <see cref="EntityWaitTimeout"/>.
    /// Completes early when <see cref="RegisterPlayer"/> / user / npc / item runs.
    /// </summary>
    public Task<Entity> GetEntityAsync(int id) =>
        GetEntityAsync(id, EntityWaitTimeout, CancellationToken.None);

    public Task<Entity> GetEntityAsync(int id, CancellationToken cancellationToken) =>
        GetEntityAsync(id, EntityWaitTimeout, cancellationToken);

    public Task<Entity> GetEntityAsync(int id, TimeSpan timeout, CancellationToken cancellationToken)
    {
        return _entityWait.WaitAsync(
            id,
            GetEntityNoLockSync,
            timeout,
            cancellationToken);
    }

    // Execute action after entity is loaded
    private async Task ExecuteWithEntityAsync(int id, Action<Entity> action) {
        Entity entity;
        try {
            entity = await GetEntityAsync(id);
        } catch (OperationCanceledException) {
            return;
        }

        if (entity == null)
            return;

        try {
            _eventProcessor.QueueEvent(() => {
                // Entity may have been removed between wait completion and main-thread apply.
                if (GetEntityNoLockSync(id) != entity)
                    return;
                action(entity);
            });
        } catch (Exception ex) {
            Debug.LogWarning($"[World] ExecuteWithEntityAsync id={id}: {ex.Message}");
        }
    }

    // Execute action after 2 entities are loaded
    private async Task ExecuteWithEntitiesAsync(int id1, int id2, Action<Entity, Entity> action) {
        Entity entity1;
        Entity entity2;
        try {
            Task<Entity> entity1Task = GetEntityAsync(id1);
            Task<Entity> entity2Task = GetEntityAsync(id2);
            await Task.WhenAll(entity1Task, entity2Task);
            entity1 = await entity1Task;
            entity2 = await entity2Task;
        } catch (OperationCanceledException) {
            return;
        }

        if (entity1 == null || entity2 == null)
            return;

        try {
            _eventProcessor.QueueEvent(() => {
                if (GetEntityNoLockSync(id1) != entity1 || GetEntityNoLockSync(id2) != entity2)
                    return;
                action(entity1, entity2);
            });
        } catch (Exception ex) {
            Debug.LogWarning($"[World] ExecuteWithEntitiesAsync id1={id1} id2={id2}: {ex.Message}");
        }
    }

    private bool IsWorldSpawnReady()
    {
        GameManager manager = _gameManager != null ? _gameManager : WorldPacketServices.Manager;
        WorldPacketApply apply = WorldPacketServices.WorldApply;
        return manager != null && manager.WorldSpawnReady && apply != null;
    }
}
