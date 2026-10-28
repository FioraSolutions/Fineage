using System;
using UnityEngine;

/// <summary>
/// Routes <see cref="IZoneTransport"/> calls based on <see cref="FineageWireConfig.Current"/>.
/// Keeps protocol mode checks out of GameManager, GameClient, and LoginWindow.
/// </summary>
public sealed class FineageZoneTransport : IZoneTransport
{
    public event Action OnDisconnected;

    public bool BypassesInterludeLogin => FineageWireConfig.IsZoneV2;

    public bool AllowsInterludeGameSocketAuth => !FineageWireConfig.IsZoneV2;

    public bool Connected
    {
        get
        {
            ZoneV2Client zone = ActiveZoneClient;
            return zone != null && zone.Connected;
        }
    }

    public int SessionObjectId
    {
        get
        {
            ZoneV2Client zone = ActiveZoneClient;
            return zone != null ? zone.SessionObjectId : 0;
        }
    }

    public bool TrySendGameCommand(INetworkCommand command)
    {
        if (!FineageWireConfig.IsZoneV2)
            return false;

        MoveToCommand move = command as MoveToCommand;
        if (move != null)
        {
            ZoneV2Client zone = ZoneV2Client.Ensure();
            zone.SendMove(move.From, move.To);
            return true;
        }

        ClickActionCommand action = command as ClickActionCommand;
        if (action != null)
        {
            World world = WorldApplyIncomingPacketServices.GameWorld;
            if (world != null)
            {
                Entity clicked = world.GetEntityNoLockSync(action.ObjectId);
                if (clicked is ItemEntity)
                {
                    ZoneV2Client.Ensure().SendPickupRequest(action.ObjectId);
                    return true;
                }
            }

            ZoneV2Client zone = ZoneV2Client.Ensure();
            zone.SendSelectTarget(action.ObjectId);
            TargetManager targets = ControllerPacketServices.Targets;
            if (targets != null && targets.IsAttackTargetSet())
                zone.SendAttackRequest(action.ObjectId);
            return true;
        }

        ZonePickupRequestCommand pickup = command as ZonePickupRequestCommand;
        if (pickup != null)
        {
            ZoneV2Client.Ensure().SendPickupRequest(pickup.GroundObjectId);
            return true;
        }

        RequestSay2Command say = command as RequestSay2Command;
        if (say != null)
        {
            if (!string.IsNullOrEmpty(say.TargetName))
                return true;
            int chatType = say.Data != null ? say.Data.Id : 0;
            ZoneV2Client.Ensure().SendSay(say.Message, chatType);
            return true;
        }

        ZoneGrantItemCommand grant = command as ZoneGrantItemCommand;
        if (grant != null)
        {
            ZoneV2Client.Ensure().SendGrantItemRequest(grant.ItemId, grant.Count);
            return true;
        }

        RequestJoinPartyCommand joinParty = command as RequestJoinPartyCommand;
        if (joinParty != null)
        {
            ZoneV2Client.Ensure().SendPartyInviteRequest(joinParty.TargetObjectId, joinParty.ItemDistribution);
            return true;
        }

        RequestAnswerJoinPartyCommand answerParty = command as RequestAnswerJoinPartyCommand;
        if (answerParty != null)
        {
            if (answerParty.RequestorObjectId > 0)
                ZoneV2Client.Ensure().SendPartyAnswerJoinRequest(answerParty.RequestorObjectId, answerParty.Response);
            return true;
        }

        if (command is RequestWithdrawalPartyCommand)
        {
            ZoneV2Client.Ensure().SendPartyLeaveRequest();
            return true;
        }

        RequestOustPartyMemberCommand oustParty = command as RequestOustPartyMemberCommand;
        if (oustParty != null)
        {
            ZoneV2Client.Ensure().SendPartyKickRequest(oustParty.TargetObjectId);
            return true;
        }

        RequestUserCommandCommand userCommand = command as RequestUserCommandCommand;
        if (userCommand != null)
        {
            if (TrySendPartyUserCommand(userCommand.Id))
                return true;
        }

        // Other Interlude game commands are no-ops on ZoneV2 until wired.
        return true;
    }

    static bool TrySendPartyUserCommand(int commandId)
    {
        int targetId = ResolveSelectedTargetObjectId();
        switch (commandId)
        {
            case 16: // PlayerCommand.INVITE — /Invite
                if (targetId <= 0)
                    return true;
                ZoneV2Client.Ensure().SendPartyInviteRequest(targetId, 0);
                return true;
            case 17: // PlayerCommand.LEAVE — /Leave
                ZoneV2Client.Ensure().SendPartyLeaveRequest();
                return true;
            case 18: // PlayerCommand.DISMISS — leader kick selected member
                if (targetId <= 0)
                    return true;
                ZoneV2Client.Ensure().SendPartyKickRequest(targetId);
                return true;
            default:
                return false;
        }
    }

    static int ResolveSelectedTargetObjectId()
    {
        TargetManager targets = ControllerPacketServices.Targets;
        if (targets == null)
            return 0;
        TargetData target = targets.Target ?? targets.AttackTarget;
        if (target?.Identity != null)
            return target.Identity.Id;
        return 0;
    }

    public void BeginSession(string account, string password)
    {
        if (!FineageWireConfig.IsZoneV2)
            return;

        ZoneV2Client.Ensure().BeginSession(account, password);
    }

    public void SendMove(Vector3 fromUnity, Vector3 toUnity)
    {
        if (!FineageWireConfig.IsZoneV2)
            return;

        ZoneV2Client.Ensure().SendMove(fromUnity, toUnity);
    }

    public void Disconnect()
    {
        if (!FineageWireConfig.IsZoneV2)
            return;

        ZoneV2Client zone = ZoneV2Client.Instance;
        if (zone != null)
            zone.Disconnect();
    }

    private static ZoneV2Client ActiveZoneClient
    {
        get { return FineageWireConfig.IsZoneV2 ? ZoneV2Client.Instance : null; }
    }
}
