using System;
using UnityEngine;
using VContainer;

public partial class TargetManager
{
    private Entity GetNextObject(World world, int kind, float radius, float minDistanceExclusive, int excludeId)
    {
        _scanKind = kind;
        _scanSelfId = DomainPacketServices.LocalPlayer.Identity.Id;
        _scanExcludeId = excludeId;
        _scanMinDistance = minDistanceExclusive;
        _scanBestDistance = radius;
        _scanOrigin = _playerTransform.position;
        _scanBest = null;
        world.ForEachEntity(_considerCandidate);
        return _scanBest;
    }

    private void ConsiderCandidate(Entity entity)
    {
        if (entity == null || entity.Identity == null || entity.transform == null)
        {
            return;
        }

        int id = entity.Identity.Id;
        if (id == _scanSelfId || id == _scanExcludeId)
        {
            return;
        }

        if (!MatchesKind(_scanKind, entity))
        {
            return;
        }

        if (entity.IsDead())
        {
            return;
        }

        float dist = VectorUtils.Distance2D(_scanOrigin, entity.transform.position);
        if (dist <= _scanMinDistance || dist >= _scanBestDistance)
        {
            return;
        }

        _scanBest = entity;
        _scanBestDistance = dist;
    }

    private static bool MatchesKind(int kind, Entity entity)
    {
        switch (kind)
        {
            case KindItem:
                return entity is ItemEntity;
            case KindNpc:
                return entity is NpcEntity;
            case KindCreature:
                return entity is MonsterEntity;
            case KindEnemy:
            case KindFriend:
            case KindParty:
                return entity is UserEntity;
            default:
                return entity is UserEntity || entity is NpcEntity || entity is MonsterEntity;
        }
    }

    private void SendAction(Entity entity)
    {
        if (entity == null || entity.Identity == null)
        {
            return;
        }

        IZoneTransport game = ZoneCommands.Transport;
        if (game == null)
        {
            return;
        }

        var l2jpos = entity.Identity.GetL2jPos();
        ZoneCommands.Send(new ClickActionCommand(
            entity.Identity.Id, (int)l2jpos.x, (int)l2jpos.y, (int)l2jpos.z, 0));
    }
}
