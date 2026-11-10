using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public partial class World
{
    public float GetGroundHeight(Vector3 pos) {
        return GroundSnapHelper.SnapToGroundOrKeep(pos, _groundMask).y;
    }

    public string GetEntityName(int id)
    {
        if (_npcs.ContainsKey(id))
        {
            return _npcs[id].name;
        }else if (_players.ContainsKey(id))
        {
            return _players[id].name;
        }
        return "";
    }

    public async Task UpdateObjectPosition(int id, Vector3 position) {
            Entity entity = await GetEntityNoLock(id);

            if(entity != null)
            {
                entity.transform.position = position;
            }
            else
            {

                if(entity != null)
                {
                    entity.transform.position = position;
                    // Debug.Log("UpdateObjectPosition not found obj. Name " + entity.name);
                }

            }
            
        //});
    }

    public Task TeleportTo(int id, Vector3 position)
    {
        return ExecuteWithEntityAsync(id, entity => {
            if (entity.GetType() == typeof(PlayerEntity))
            {
                PlayerTeleport teleport = entity.GetComponent<PlayerTeleport>();
                teleport.TeleportTo(position);
                Vector3 grounded = teleport.LastTeleportPosition;
                // Match original client after TeleportToLocationDto / L2_Teleport:
                // ValidatePosition then Appearing → server onTeleported() + UserInfoDto / knownlist.
                SendValidatePosition(grounded, PlayerPositionSender.ToL2Heading(entity.transform.eulerAngles.y));
                SendAppearing();
            }
        });
    }

    private void SendValidatePosition(Vector3 position, int heading)
    {
        IZoneTransport game = ZoneCommands.Transport;
        if (game != null)
            ZoneCommands.Send(new ValidatePositionCommand(position.x, position.y, position.z, heading));
    }

    private void SendAppearing()
    {
        IZoneTransport game = ZoneCommands.Transport;
        if (game != null)
            ZoneCommands.Send(new AppearingCommand());
    }

    public Task UpdateObjectRotation(int id, float angle) {
        return ExecuteWithEntityAsync(id, e => { });
    }

    public Task UpdateObjectDestination(int id, Vector3 position, int speed, bool walking) {
        return ExecuteWithEntityAsync(id, e => {
            if (speed != e.Stats.Speed) {
                e.UpdateSpeed(speed);
            }

            e.OnStartMoving(walking);
        });
    }

    public Task UpdateObjectAnimation(int id, int animId, float value) {
        return ExecuteWithEntityAsync(id, e => {
            e.GetComponent<NetworkAnimationController>().SetAnimationProperty(animId, value);
        });
    }

    public Task InflictDamageTo(int sender, int target, int damage, bool criticalHit) {
        return ExecuteWithEntitiesAsync(sender, target, (senderEntity, targetEntity) => {
            if (senderEntity != null) {
                //WorldCombat.Instance.InflictAttack(senderEntity.transform, targetEntity.transform, damage, criticalHit);
            } else {
                if (_worldCombat != null)
                    _worldCombat.InflictAttack(targetEntity.transform, damage, criticalHit);
            }
        });
    }

    public Task UpdateObjectMoveDirection(int id, int speed, Vector3 direction) {
        return ExecuteWithEntityAsync(id, e => {
            if (speed != e.Stats.Speed) {
                e.UpdateSpeed(speed);
            }
            // Movement direction applied via MoveAllCharacters / CharMoveToLocationDto.
        });
    }

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
