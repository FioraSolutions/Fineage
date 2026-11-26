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
    public void RegisterPlayer(IWorldEntity player)
    {
        PlayerEntity entity = player as PlayerEntity;
        if (entity == null)
        {
            if (player == null || player is PlayerEntity)
                return;

            throw new InvalidOperationException(
                "[World] IWorldSpawnRegistration.RegisterPlayer is not a PlayerEntity — the spawner passed a different type, so the local player was not registered.");
        }

        if (entity.Identity == null)
            return;

        int id = entity.Identity.Id;
        _entities.RegisterPlayer(id, entity);
        _entityWait.Notify(id, entity);
        L2ActorPointLights.TryRegisterActorRoot(entity.transform);
        _visibility.Register(id, entity.transform, isLocalPlayer: true);
    }

    public void RegisterUser(IWorldEntity user)
    {
        Entity entity = user as Entity;
        if (entity == null)
        {
            if (user == null || user is Entity)
                return;

            throw new InvalidOperationException(
                "[World] IWorldSpawnRegistration.RegisterUser is not an Entity — the spawner passed a different type, so the user was not registered.");
        }

        if (entity.Identity == null)
            return;

        int id = entity.Identity.Id;
        if (!_entities.TryRegisterUser(id, entity))
        {
            _entityWait.Notify(id, _entities.GetOrNull(id));
            return;
        }

        _entityWait.Notify(id, entity);
        L2ActorPointLights.TryRegisterActorRoot(entity.transform);
        _visibility.Register(id, entity.transform);
    }

    public void SpawnUser(CharInfoDto info)
    {
        if (info == null || info.Identity == null || _userSpawner == null)
        {
            // GearFlowLog.Warn("SpawnUser abort info/spawner null");
            return;
        }

        int id = info.Identity.Id;
        if (_entities.Contains(id))
        {
            // GearFlowLog.Info("SpawnUser SKIP already in world id=" + id +
                // " type=" + _objects[id].GetType().Name);
            return;
        }

        if (_localPlayer.IsLocalId(id))
        {
            // GearFlowLog.Info("SpawnUser SKIP local PlayerEntity id=" + id);
            return;
        }

        // GearFlowLog.Info("SpawnUser CREATE UserEntity id=" + id +
            // " nick=" + info.Identity.Name + " " + GearFlowLog.Paperdoll(info.Appearance));
        _userSpawner.Spawn(info, this);
    }
}
