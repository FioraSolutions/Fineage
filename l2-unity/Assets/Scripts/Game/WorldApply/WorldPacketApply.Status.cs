using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class WorldPacketApply
{
    public void UpdateUser(CharInfoDto info)
    {
        if (GameWorld == null || info == null || info.Identity == null)
        {
            // GearFlowLog.Warn("UpdateUser abort world/info null");
            return;
        }
        if (!IsWorldSpawnReady())
        {
            // GearFlowLog.Warn("UpdateUser abort world not ready id=" + info.Identity.Id +
                // " nick=" + info.Identity.Name + " " + GearFlowLog.Paperdoll(info.Appearance));
            return;
        }

        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local != null && local.Identity != null && local.Identity.Id == info.Identity.Id)
        {
            // GearFlowLog.Info("UpdateUser SKIP local id=" + info.Identity.Id);
            return;
        }

        Entity entity = GameWorld.GetEntityNoLockSync(info.Identity.Id);
        if (entity == null)
        {
            if (SpawnFrameBudget.IsCancelled(info.Identity.Id))
                return;
            if (!SpawnFrameBudget.TryConsumeSpawn())
            {
                QueueWorldNextFrame(apply => apply.UpdateUser(info));
                return;
            }
            GameWorld.SpawnUser(info);
            _gameEvents?.Publish(new EntitySpawnedEvent(info.Identity.Id));
            // PartySpelled may have arrived before CharInfo finished spawning the peer.
            AbnormalStatusUpdateApplyIncomingPacketServices.FlushPendingPeerAbnormals(info.Identity.Id);
            return;
        }

        if (entity is UserEntity)
        {
            // GearFlowLog.Info("UpdateUser APPLY UserEntity id=" + info.Identity.Id +
                // " nick=" + info.Identity.Name +
                // " type=" + entity.GetType().Name +
                // " have " + GearFlowLog.Paperdoll(entity) +
                // " want " + GearFlowLog.Paperdoll(info.Appearance));
            GameWorld.UpdateUser(entity, info);
            return;
        }

        // GearFlowLog.Warn("UpdateUser SKIP not UserEntity id=" + info.Identity.Id +
            // " type=" + entity.GetType().Name);
    }

    public void ShowNpcHtml(NpcHtmlMessageDto dto)
    {
        if (dto.GetNpcId() == 0 && _html != null)
        {
            _html.InjectToWindow(dto.Html);
            _html.ShowWindowToCenterAndBringToFront();
        }

        Entity npc = GetEntity(dto.GetNpcId());
        EntityWorldApply apply;
        if (npc == null || !TryResolve(npc, out apply))
            return;

        apply.OnNpcHtml(npc, dto);
    }

    public void SendArrivedPosition()
    {
        PlayerController player = WorldApplyPacketServices.Player;
        if (player == null)
            return;

        _positionSender.SendServerArrivedPosition(player.transform.position);
    }

    private Entity GetEntity(int objId)
    {
        return GameWorld == null ? null : GameWorld.GetEntityNoLockSync(objId);
    }

    private bool TryResolve(Entity entity, out EntityWorldApply apply)
    {
        Type type = entity.GetType();
        while (type != null && type != typeof(Entity) && type != typeof(object))
        {
            if (_byType.TryGetValue(type, out apply))
                return true;
            type = type.BaseType;
        }

        apply = null;
        return false;
    }

    private bool IsWorldSpawnReady()
    {
        GameManager manager = _gameManager != null ? _gameManager : WorldApplyPacketServices.Manager;
        return manager != null && manager.WorldSpawnReady && GameWorld != null;
    }

    private void QueueWorldNextFrame(System.Action<WorldPacketApply> apply)
    {
        if (apply == null)
            return;
        if (_applyQueue != null)
            _applyQueue.QueueNextFrame(() => apply(this));
        else
            WorldApplyPacketServices.QueueWorldNextFrame(apply);
    }
}
