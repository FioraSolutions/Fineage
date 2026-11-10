using System.Collections.Generic;
using UnityEngine;

public sealed class RequestWithdrawPledgeCommand : INetworkCommand
{
}

/// <summary>Fineage Net TX opcode 32. Body key 0 is the clan name.</summary>
public sealed class ClanCreateRequestCommand : INetworkCommand
{
    public string Name;

    public ClanCreateRequestCommand(string name)
    {
        Name = name ?? "";
    }
}

/// <summary>Fineage Net TX opcode 33. Pledge type 0 is the main clan.</summary>

/// <summary>Fineage Net TX opcode 33. Pledge type 0 is the main clan.</summary>
public sealed class ClanInviteRequestCommand : INetworkCommand
{
    public int TargetObjectId;
    public int PledgeType;

    public ClanInviteRequestCommand(int targetObjectId, int pledgeType = 0)
    {
        TargetObjectId = targetObjectId;
        PledgeType = pledgeType;
    }
}

/// <summary>Fineage Net TX opcode 34. Response 0 declines. Any other value accepts.</summary>

/// <summary>Fineage Net TX opcode 34. Response 0 declines. Any other value accepts.</summary>
public sealed class ClanAnswerJoinRequestCommand : INetworkCommand
{
    public int RequestorObjectId;
    public int Response;

    public ClanAnswerJoinRequestCommand(int requestorObjectId, int response)
    {
        RequestorObjectId = requestorObjectId;
        Response = response;
    }
}

/// <summary>Fineage Net TX opcode 35. Empty body.</summary>

/// <summary>Fineage Net TX opcode 35. Empty body.</summary>
public sealed class ClanLeaveRequestCommand : INetworkCommand
{
}

/// <summary>Fineage Net TX opcode 36.</summary>

/// <summary>Fineage Net TX opcode 36.</summary>
public sealed class ClanKickRequestCommand : INetworkCommand
{
    public int TargetObjectId;

    public ClanKickRequestCommand(int targetObjectId)
    {
        TargetObjectId = targetObjectId;
    }
}

/// <summary>Fineage Net TX opcode 37. Empty body.</summary>

/// <summary>Fineage Net TX opcode 37. Empty body.</summary>
public sealed class ClanDissolveRequestCommand : INetworkCommand
{
}

/// <summary>Fineage Net TX opcode 38. Empty body.</summary>

/// <summary>Fineage Net TX opcode 38. Empty body.</summary>
public sealed class ClanCancelDissolveRequestCommand : INetworkCommand
{
}

public sealed class RequestJoinPledgeCommand : INetworkCommand
{
    public int ObjectId;

    public RequestJoinPledgeCommand(int objectId)
    {
        ObjectId = objectId;
    }
}

public sealed class RequestGiveNickNameCommand : INetworkCommand
{
    public string MemberName;
    public string Title;

    public RequestGiveNickNameCommand(string memberName, string title)
    {
        MemberName = memberName;
        Title = title;
    }
}

public sealed class RequestOustPledgeMemberCommand : INetworkCommand
{
    public string MemberName;

    public RequestOustPledgeMemberCommand(string memberName)
    {
        MemberName = memberName;
    }
}

public sealed class RequestPledgePowerCommand : INetworkCommand
{
    public int Rank;
    public int Action;
    public int Privs;

    public RequestPledgePowerCommand(int rank, int action, int privs)
    {
        Rank = rank;
        Action = action;
        Privs = privs;
    }
}

public sealed class RequestPledgeInfoCommand : INetworkCommand
{
    public int ClanId;

    public RequestPledgeInfoCommand(int clanId)
    {
        ClanId = clanId;
    }
}

