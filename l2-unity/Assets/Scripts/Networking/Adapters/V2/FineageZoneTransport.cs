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

        ZoneGrantItemCommand grant = command as ZoneGrantItemCommand;
        if (grant != null)
        {
            ZoneV2Client.Ensure().SendGrantItemRequest(grant.ItemId, grant.Count);
            return true;
        }

        UseItemCommand useItem = command as UseItemCommand;
        if (useItem != null)
        {
            ZoneV2Client.Ensure().SendUseItem(useItem.ObjectId);
            return true;
        }

        ZoneEnchantRequestCommand enchantRequest = command as ZoneEnchantRequestCommand;
        if (enchantRequest != null)
        {
            ZoneV2Client.Ensure().SendEnchantRequest(enchantRequest.ScrollObjectId);
            return true;
        }

        RequestEnchantItemCommand enchantItem = command as RequestEnchantItemCommand;
        if (enchantItem != null)
        {
            if (enchantItem.ObjectId < 0)
                ZoneV2Client.Ensure().SendEnchantCancel();
            else
                ZoneV2Client.Ensure().SendEnchantConfirm(enchantItem.ObjectId);
            return true;
        }

        UnequipItemCommand unequip = command as UnequipItemCommand;
        if (unequip != null)
        {
            ZoneV2Client.Ensure().SendUnequipItem(unequip.PaperdollSlot);
            return true;
        }

        RequestSay2Command say = command as RequestSay2Command;
        if (say != null)
        {
            int chatType = say.Data != null ? say.Data.Type : ZoneV2Chat.All;
            ZoneV2Client.Ensure().SendSay(say.Message, chatType);
            return true;
        }

        RequestMagicSkillUseCommand skillUse = command as RequestMagicSkillUseCommand;
        if (skillUse != null)
        {
            int targetId = ResolveSelectedTargetObjectId();
            bool ctrl = skillUse.CtrlPressed != 0;
            ZoneV2Client.Ensure().SendSkillUseRequest(skillUse.SkillId, targetId, ctrl);
            return true;
        }

        RequestBypassToServerCommand bypass = command as RequestBypassToServerCommand;
        int giveItemId;
        int giveCount;
        if (bypass != null && ZoneV2GiveCommand.TryParse(bypass.Bypass, out giveItemId, out giveCount))
        {
            ZoneV2Client.Ensure().SendGiveItem(giveItemId, giveCount);
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

        ClanCreateRequestCommand createClan = command as ClanCreateRequestCommand;
        if (createClan != null)
        {
            ZoneV2Client.Ensure().SendClanCreateRequest(createClan.Name);
            return true;
        }

        ClanInviteRequestCommand inviteClan = command as ClanInviteRequestCommand;
        if (inviteClan != null)
        {
            ZoneV2Client.Ensure().SendClanInviteRequest(inviteClan.TargetObjectId, inviteClan.PledgeType);
            return true;
        }

        RequestJoinPledgeCommand joinPledge = command as RequestJoinPledgeCommand;
        if (joinPledge != null)
        {
            ZoneV2Client.Ensure().SendClanInviteRequest(joinPledge.ObjectId, ZoneV2ClanBook.MainPledge);
            return true;
        }

        ClanAnswerJoinRequestCommand answerClan = command as ClanAnswerJoinRequestCommand;
        if (answerClan != null)
        {
            if (answerClan.RequestorObjectId > 0)
                ZoneV2Client.Ensure().SendClanAnswerJoinRequest(answerClan.RequestorObjectId, answerClan.Response);
            return true;
        }

        if (command is ClanLeaveRequestCommand || command is RequestWithdrawPledgeCommand)
        {
            ZoneV2Client.Ensure().SendClanLeaveRequest();
            return true;
        }

        ClanKickRequestCommand kickClan = command as ClanKickRequestCommand;
        if (kickClan != null)
        {
            ZoneV2Client.Ensure().SendClanKickRequest(kickClan.TargetObjectId);
            return true;
        }

        RequestOustPledgeMemberCommand oustPledge = command as RequestOustPledgeMemberCommand;
        if (oustPledge != null)
        {
            ZoneV2PledgeMember member;
            if (ZoneV2ClanBook.Shared.TryMemberByName(oustPledge.MemberName, out member))
                ZoneV2Client.Ensure().SendClanKickRequest(member.ObjectId);
            return true;
        }

        if (command is ClanDissolveRequestCommand)
        {
            ZoneV2Client.Ensure().SendClanDissolveRequest();
            return true;
        }

        if (command is ClanCancelDissolveRequestCommand)
        {
            ZoneV2Client.Ensure().SendClanCancelDissolveRequest();
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
