using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public partial class World
{
    public float GetGroundHeight(Vector3 pos) {
        return WorldPlacement.Resolve(WorldPlacement.PlacementKind.WalkValidate, pos, _groundMask).y;
    }

    /// <summary>
    /// Iterates registered NPCs/monsters (nameplates when OverlapSphere misses EntityClick).
    /// </summary>
    public void ForEachNpc(System.Action<Entity> action)
    {
        if (action == null)
            return;

        foreach (KeyValuePair<int, Entity> pair in _npcs)
        {
            if (pair.Value != null)
                action(pair.Value);
        }
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

}
