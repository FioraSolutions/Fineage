using UnityEngine.Animations;
using UnityEngine.Playables;
using System;
using UnityEngine;

public sealed partial class CastClipPlayable
{
    private void SwitchToIdle()
    {
        MagicGaitTrace.EventId(_objectId, "CastClip.SwitchToIdle",
            "local=" + EntityActionSkill.IsLocalPlayer(_objectId));
        if (!EntityActionSkill.IsLocalPlayer(_objectId))
        {
            EntityActionSkill.FinishRemoteCast(_objectId);
            return;
        }

        if (DomainPacketServices.LocalPlayer != null)
        {
            DomainPacketServices.LocalPlayer.IsAttack = false;
        }

        if (PlayerStatePacketServices.PlayerState != null && PlayerStatePacketServices.PlayerState.ShouldKeepLocomotion())
        {
            return;
        }

        if (PlayerStatePacketServices.PlayerState == null)
        {
            return;
        }

        PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
        PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN, NewIdleState.WaitReturnFromCombatSmb);
    }

    private static int StateIndex(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName))
        {
            return 1;
        }

        if (triggerName.StartsWith("CastMid", System.StringComparison.Ordinal))
        {
            return 0;
        }

        if (triggerName.StartsWith("MagicShot", System.StringComparison.Ordinal))
        {
            return 2;
        }

        return 1;
    }

    private static float PhaseSpeed(MagicCastData castData, int stateIndex)
    {
        if (castData == null)
        {
            return 1f;
        }

        if (stateIndex == 0)
        {
            return castData.SpeedMid;
        }

        if (stateIndex == 1)
        {
            return castData.SpeedEnd;
        }

        return castData.SpeedShot;
    }
}
