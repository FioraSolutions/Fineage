using System;
using UnityEngine;

public sealed partial class UserSpawner
{
    public void UpdateInfo(Entity entity, CharInfoDto info)
    {
        if (entity == null || info == null || !(entity is UserEntity))
            return;

        UserEntity user = (UserEntity)entity;
        EntityIdentity identity = info.Identity;
        user.Identity.Position = identity.Position;
        user.Identity.Heading = identity.Heading;
        user.Identity.OrigHeading = identity.OrigHeading;
        user.Identity.Name = identity.Name;
        user.Identity.Title = identity.Title;
        user.Identity.PlayerClass = identity.PlayerClass;
        user.Identity.IsRunning = identity.IsRunning;
        user.Identity.ClanId = identity.ClanId;

        // GearFlowLog.Info("UserSpawner.UpdateInfo BEFORE nick=" + user.Nick +
            // " id=" + (user.Identity != null ? user.Identity.Id : 0) +
            // " " + GearFlowLog.Paperdoll(user));
        // GearFlowLog.Info("UserSpawner.UpdateInfo PACKET nick=" + identity.Name +
            // " id=" + identity.Id + " " + GearFlowLog.Paperdoll(info.Appearance));

        bool wasCorpse = user.GetDead();
        user.Status = info.Status;
        user.Stats = info.Stats;
        user.Appearance = info.Appearance;
        if (user.Identity != null)
            L2OverheadAnchor.Invalidate(user.Identity.Id);
        user.Running = info.Appearance.Running;
        user.SetDead(info.AlikeDead);

        user.UpdateRunSpeed(info.Stats.RunRealSpeed);
        user.UpdateWalkSpeed(info.Stats.WalkRealSpeed);
        user.UpdatePAtkSpeed((int)info.Stats.PAtkSpd);
        user.UpdateMAtkSpeed((int)info.Stats.MAtkSpd);
        user.LogSpeed("update CharInfo RunReal=" + info.Stats.RunRealSpeed +
            " WalkReal=" + info.Stats.WalkRealSpeed);
        // CharInfoSpeedLog.LogPacket(user, "OnCharInfo update");
        // CharInfoSpeedLog.LogSnap(user, identity.Position, "CharInfo UpdateInfo");
        // CharInfoMoveBudgetLog.Compare(user, "CHARINFO", EntityPawnLookup.PawnOf(user), identity.Position, true);
        // GearFlowLog.Info("UserSpawner.RefreshVisuals nick=" + user.Nick);
        if (_appearFade != null && info.AlikeDead && user.Identity != null)
            _appearFade.Cancel(user.Identity.Id);

        if (_appearFade != null && !info.AlikeDead)
            _appearFade.AroundVisualRefresh(user, user.RefreshVisuals);
        else
            user.RefreshVisuals();

        float snap2d = VectorUtils.Distance2D(user.transform.position, identity.Position);
        float skipM = VectorUtils.ConvertL2UuToMeters(L2PawnRange.AdjustSkipUu);
        if (snap2d > skipM)
        {
            Vector3 fromL2 = VectorUtils.ConvertPosUnityToL2j(user.transform.position);
            Vector3 toL2 = VectorUtils.ConvertPosUnityToL2j(identity.Position);
            // Debug.Log("[MOVE_SNAP] CHARINFO nick=" + user.Nick +
                // " snapM=" + snap2d.ToString("F2") +
                // " skipM=" + skipM.ToString("F2") +
                // " fromL2=" + fromL2.x.ToString("F0") + "," + fromL2.y.ToString("F0") + "," + fromL2.z.ToString("F0") +
                // " toL2=" + toL2.x.ToString("F0") + "," + toL2.y.ToString("F0") + "," + toL2.z.ToString("F0"));
            EntitySpawnShared.ApplyGroundedTransform(user.gameObject, identity.Position, identity.Heading);
            EntitySpawnShared.ReapplyGroundAfterActivate(user.gameObject, user.Identity);
        }
        else
        {
            _pawnYaw.RequestYaw(user.transform, identity.Heading.eulerAngles.y, L2PawnYaw.RotationRateYaw);
            // Debug.Log("[CI_MOVE] CHARINFO SKIP_SNAP nick=" + user.Nick +
                // " snap2d=" + snap2d.ToString("F3") +
                // " skipM=" + skipM.ToString("F2") +
                // " (AdjustPawnLocation <= 200 UU)");
        }

        if (info.AlikeDead)
        {
            // EntityActionCombatLog.LogIfWatch(user,
                // "CharInfo dead nick=" + user.Nick +
                // " wasCorpse=" + wasCorpse +
                // " alikeDead=" + info.AlikeDead);
            if (wasCorpse)
                return;
            if (EntityActionPacketServices.Actions != null)
                EntityActionPacketServices.Actions.Die(user, false);
            else
                EntityActionVisual.PlayDeath(user, false);
        }
        else if (wasCorpse && EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.Revive(user);
    }

    static void ResetRuntime(UserEntity user)
    {
        if (user == null)
            return;

        EntitySpawnShared.ResetNpcRuntimeForSpawn(user);
        user.ClearAttackVisual();
        NetworkAnimationController animation = user.GetAnimatorController();
        if (animation != null)
            animation.enabled = true;
    }
}
