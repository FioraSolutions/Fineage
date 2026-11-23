using System;
using UnityEngine;
using VContainer;

public partial class TargetManager
{
    public void SetMask(LayerMask mask)
    {
        _entityMask = mask;
    }

    void Update()
    {
        if (DomainPacketServices.LocalPlayer == null || !HasTarget())
        {
            return;
        }

        // Target gone from the world (destroyed, pooled after corpse removal,
        // or hidden like a ride pet): drop the selection immediately so the
        // TargetWindow/nameplate bubble do not keep a dead/removed entity.
        if (IsTargetGone())
        {
            ClearTarget();
            return;
        }

        PlayerController player = ControllerPacketServices.Player;
        if (player == null)
        {
            return;
        }

        _target.Distance = VectorUtils.Distance2D(
            player.transform.position,
            _target.Data.ObjectTransform.position);
    }

    private bool IsTargetGone()
    {
        if (_target == null || _target.Data == null)
        {
            return true;
        }

        Transform t = _target.Data.ObjectTransform;
        if (t == null)
        {
            return true;
        }

        Entity entity = _target.GetEntity();
        if (entity == null)
        {
            return true;
        }

        // Pooled / hidden (SetActive(false)) objects are no longer in the world.
        if (!t.gameObject.activeInHierarchy)
        {
            return true;
        }

        // Local player is never registered in the spawned catalog;
        // anything else must still be in that catalog.
        RequireSpawned();
        int id = entity.Identity != null ? entity.Identity.Id : 0;
        if (entity != DomainPacketServices.LocalPlayer && _spawned.Find(id) == null)
        {
            return true;
        }

        return false;
    }

    private bool TryResolvePlayerTransform()
    {
        if (_playerTransform != null)
        {
            return true;
        }

        if (DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.transform != null)
        {
            _playerTransform = DomainPacketServices.LocalPlayer.transform;
            return true;
        }

        return false;
    }
}
