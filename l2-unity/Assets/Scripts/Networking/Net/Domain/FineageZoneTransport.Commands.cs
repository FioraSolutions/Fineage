using Fineage.Net.Contract;
using UnityEngine;
using System;

public sealed partial class FineageZoneTransport
{
    public bool TrySendGameCommand(INetworkCommand command)
    {
        if (TrySendWorldCommands(command))
            return true;
        if (TrySendInteractionCommands(command))
            return true;
        if (TrySendEconomySocialCommands(command))
            return true;
        return false;
    }

    static bool TrySendNetPartyUserCommand(int commandId)
    {
        int targetId = ResolveSelectedTargetObjectId();
        switch (commandId)
        {
            case 16:
                if (targetId <= 0)
                    return true;
                Session.SendPartyInviteRequest(targetId, 0);
                return true;
            case 17:
                Session.SendPartyLeaveRequest();
                return true;
            case 18:
                if (targetId <= 0)
                    return true;
                Session.SendPartyKickRequest(targetId);
                return true;
            default:
                return false;
        }
    }

    static bool TrySendNetTradeUserCommand(int commandId)
    {
        if (commandId != (int)PlayerCommand.TRADE)
            return false;
        int targetId = ResolveSelectedTargetObjectId();
        if (targetId <= 0)
            return true;
        Session.SendTradeRequest(targetId);
        return true;
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

    static bool IsAdminBypass(string bypass)
    {
        if (string.IsNullOrWhiteSpace(bypass))
            return false;
        string text = bypass.Trim();
        if (text.StartsWith("bypass -h ", System.StringComparison.OrdinalIgnoreCase))
            text = text.Substring("bypass -h ".Length).TrimStart();
        else if (text.StartsWith("bypass ", System.StringComparison.OrdinalIgnoreCase))
            text = text.Substring("bypass ".Length).TrimStart();
        if (text.StartsWith("npc_", System.StringComparison.OrdinalIgnoreCase))
        {
            int split = text.IndexOf('_', 4);
            if (split > 4)
                text = text.Substring(split + 1).TrimStart();
        }
        return text.StartsWith("admin_", System.StringComparison.OrdinalIgnoreCase);
    }
}
