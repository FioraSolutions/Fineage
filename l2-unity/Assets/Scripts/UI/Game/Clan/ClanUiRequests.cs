using Fineage.Net.Contract;
using UnityEngine;

/// <summary>Clan window (Alt+N): zone <see cref="PledgeCommandKind"/> requests and explicit client errors.</summary>
public static class ClanUiRequests
{
    public static void SendPledgeCommand(int kind, string requestName, PledgeCommandNetMsg body = null)
    {
        if (body == null)
            body = new PledgeCommandNetMsg { Kind = kind };
        else
            body.Kind = kind;
        ZoneCommands.Send(new PledgeCommandRequestCommand(body, requestName));
    }

    public static void RejectLocal(string requestName, string reason)
    {
        Debug.LogError("[ClanWindow] Request " + requestName + ": " + reason);
        NetChatFeed.NoteSystem(reason);
    }

    public static int SelectedTargetObjectId()
    {
        TargetManager targets = ControllerPacketServices.Targets;
        if (targets == null)
            return 0;
        TargetData target = targets.Target != null ? targets.Target : targets.AttackTarget;
        if (target != null && target.Identity != null)
            return target.Identity.Id;
        return 0;
    }
}