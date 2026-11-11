using System;
using UnityEngine;
using VContainer;

public partial class TargetManager
{
    public void NextTarget()
    {
        NextTarget(KindAllPawns);
    }

    public void NextTarget(int kind)
    {
        if (DomainPacketServices.LocalPlayer == null || DomainPacketServices.LocalPlayer.Identity == null)
        {
            return;
        }

        if (!TryResolvePlayerTransform())
        {
            return;
        }

        World world = GameWorld;
        if (world == null)
        {
            return;
        }

        if (kind != _cycleKind)
        {
            _cycleKind = kind;
            _cycleLastDistance = 0f;
            _cycleLastObjectId = -1;
        }
        else if (_cycleLastObjectId > 0)
        {
            Entity last = world.GetEntityNoLockSync(_cycleLastObjectId);
            if (last != null && last.transform != null)
            {
                _cycleLastDistance = VectorUtils.Distance2D(_playerTransform.position, last.transform.position);
            }
            else
            {
                _cycleLastDistance = 0f;
                _cycleLastObjectId = -1;
            }
        }

        Entity next = GetNextObject(world, kind, _maximumTargetDistance, _cycleLastDistance, _cycleLastObjectId);
        if (next == null)
        {
            next = GetNextObject(world, kind, _maximumTargetDistance, 0f, _cycleLastObjectId);
        }

        if (next == null)
        {
            next = GetNextObject(world, kind, _maximumTargetDistance, 0f, -1);
        }

        if (next == null || next.Identity == null || next.transform == null)
        {
            return;
        }

        _cycleLastObjectId = next.Identity.Id;
        _cycleLastDistance = VectorUtils.Distance2D(_playerTransform.position, next.transform.position);

        SetTarget(new ObjectData(next.gameObject), defaultHexColor);
        SendAction(next);
    }

    public void NextTargetById(int id , string hexColor)
    {
        World world = GameWorld;
        Entity found = world != null ? world.GetEntityNoLockSync(id) : null;
        if (found == null &&
            DomainPacketServices.LocalPlayer != null &&
            DomainPacketServices.LocalPlayer.Identity != null &&
            DomainPacketServices.LocalPlayer.Identity.Id == id)
        {
            found = DomainPacketServices.LocalPlayer;
        }

        if (found == null)
        {
            return;
        }

        SetTarget(new ObjectData(found.gameObject), hexColor);
    }
}
