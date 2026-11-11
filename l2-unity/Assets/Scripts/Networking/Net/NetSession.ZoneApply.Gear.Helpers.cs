using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
    internal void ApplyUserInfo(UserInfoNetMsg msg)
    {
        // The zone server sends UserInfo right after the self EntitySpawn, while the
        // world scene is still loading. NoteUserInfo stashes it and overlays the live
        // player / stashed PlayerInfo as those appear, so the paperdoll and stats are
        // not lost to the load order.
        NetUserInfo user = MapUserInfo(msg);
        NetGearSession.NoteUserInfo(user);
    }

    internal void ApplyItemList(ItemListNetMsg list)
    {
        NetItemEntry[] rows = MapItemList(list);
        NetGearSession.NoteItemList(rows);
        Debug.Log("[Net] ItemList count=" + rows.Length);
    }

    internal void ApplySkillList(SkillListNetMsg list)
    {
        NetSkillEntry[] rows = MapSkillList(list);
        SkillListDto dto = MapSkillListToDto(rows);
        if (_skills != null) _skills.ApplySkillList(dto);
        else SkillListApplyIncomingPacketServices.ApplySkillList(dto);
        Debug.Log("[Net] SkillList count=" + rows.Length);
    }

    internal void ApplyMoveState(MoveStateMsg move)
    {
        int objectId = (int)move.EntityValue;
        Vector3 dest = VectorUtils.ConvertPosToUnity(new Vector3(move.DestX, move.DestY, move.DestZ));
        Vector3 origin = VectorUtils.ConvertPosToUnity(new Vector3(move.OriginX, move.OriginY, move.OriginZ));
        var domain = new DomainMoveTo
        {
            ObjectId = objectId,
            Destination = dest,
            Origin = origin,
            // The intention layer (NewMoveToIntention) reads the Interlude DTO and bails out when it is
            // null, so the local player stayed put even though MoveState arrived. Synthesize it here.
            InterludeDto = CharMoveToLocationDto.FromUnity(objectId, dest, origin)
        };
        if (_sink != null)
            _sink.ApplyMoveTo(domain);
        else if (WorldApplyPacketServices.WorldApply != null)
            WorldApplyPacketServices.WorldApply.MoveTo(objectId, dest, origin, domain.InterludeDto);
    }

    internal void ApplyCorrection(PositionCorrectionMsg corr)
    {
        int objectId = (int)corr.EntityValue;
        Vector3 pos = VectorUtils.ConvertPosToUnity(new Vector3(corr.X, corr.Y, corr.Z));
        // Domain apply — never INTENTION_MOVE_TO with dest==origin (fights approach MoveState).
        PostCritical(() =>
        {
            if (_sink != null)
                _sink.ApplyPositionCorrection(objectId, pos);
            else if (WorldApplyPacketServices.WorldApply != null)
                WorldApplyPacketServices.WorldApply.ApplyPositionCorrection(objectId, pos);
        });
    }
}
