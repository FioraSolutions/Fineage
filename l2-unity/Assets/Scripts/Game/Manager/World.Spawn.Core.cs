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
    public void RegisterPlayer(PlayerEntity player)
    {
        if (player == null || player.Identity == null)
            return;

        int id = player.Identity.Id;
        _entities.RegisterPlayer(id, player);
        _entityWait.Notify(id, player);
    }

    public void RegisterUser(Entity user)
    {
        if (user == null || user.Identity == null)
            return;

        int id = user.Identity.Id;
        if (!_entities.TryRegisterUser(id, user))
        {
            _entityWait.Notify(id, _entities.GetOrNull(id));
            return;
        }

        _entityWait.Notify(id, user);
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

        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local != null && local.Identity != null && local.Identity.Id == id)
        {
            // GearFlowLog.Info("SpawnUser SKIP local PlayerEntity id=" + id);
            return;
        }

        // GearFlowLog.Info("SpawnUser CREATE UserEntity id=" + id +
            // " nick=" + info.Identity.Name + " " + GearFlowLog.Paperdoll(info.Appearance));
        _userSpawner.Spawn(info, this);
    }
}
