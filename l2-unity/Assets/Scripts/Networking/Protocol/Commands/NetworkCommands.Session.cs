using System.Collections.Generic;
using UnityEngine;

public sealed class AppearingCommand : INetworkCommand
{
}

public sealed class EnterWorldCommand : INetworkCommand
{
}

public sealed class NewCharacterCommand : INetworkCommand
{
}

public sealed class ProtocolVersionCommand : INetworkCommand
{
    public int Protocol;

    public ProtocolVersionCommand(int protocol)
    {
        Protocol = protocol;
    }
}

public sealed class CharacterSelectCommand : INetworkCommand
{
    public int Slot;

    public CharacterSelectCommand(int slot)
    {
        Slot = slot;
    }
}

public sealed class CharacterDeleteCommand : INetworkCommand
{
    public int Slot;

    public CharacterDeleteCommand(int slot)
    {
        Slot = slot;
    }
}

public sealed class CharacterCreateCommand : INetworkCommand
{
    public List<PlayerTemplates> Templates;
    public string ClassName;
    public string Sex;
    public string HairColor;
    public string HairStyle;
    public string Face;
    public string RaceName;
    public string Name;

    public CharacterCreateCommand(
        List<PlayerTemplates> templates,
        string className,
        string sex,
        string hairColor,
        string hairStyle,
        string face,
        string raceName,
        string name)
    {
        Templates = templates;
        ClassName = className;
        Sex = sex;
        HairColor = hairColor;
        HairStyle = hairStyle;
        Face = face;
        RaceName = raceName;
        Name = name;
    }
}

public sealed class AuthLoginCommand : INetworkCommand
{
    public string Account;
    public int PlayKey1;
    public int PlayKey2;
    public int LoginKey1;
    public int LoginKey2;

    public AuthLoginCommand(string account, int playKey1, int playKey2, int loginKey1, int loginKey2)
    {
        Account = account;
        PlayKey1 = playKey1;
        PlayKey2 = playKey2;
        LoginKey1 = loginKey1;
        LoginKey2 = loginKey2;
    }
}

public sealed class RequestRestartPointCommand : INetworkCommand
{
    /// <summary>Matches <c>ReviveRequestNetMsg</c> / <c>DeathProtocol</c> revive actions.</summary>
    public int Action;

    public RequestRestartPointCommand(int action = 0)
    {
        Action = action;
    }
}

public sealed class RequestShowBoardCommand : INetworkCommand
{
}

public sealed class AuthGameGuardCommand : INetworkCommand
{
    public int SessionId;
    public int[] Gg;

    public AuthGameGuardCommand(int sessionId, int[] gg)
    {
        SessionId = sessionId;
        Gg = gg;
    }
}

public sealed class RequestAuthLoginCommand : INetworkCommand
{
    public string Account;
    public string Password;
    public int Response;

    public RequestAuthLoginCommand(string account, string password, int response)
    {
        Account = account;
        Password = password;
        Response = response;
    }
}

public sealed class RequestServerListCommand : INetworkCommand
{
    public int SessionKey1;
    public int SessionKey2;

    public RequestServerListCommand(int sessionKey1, int sessionKey2)
    {
        SessionKey1 = sessionKey1;
        SessionKey2 = sessionKey2;
    }
}

public sealed class RequestServerLoginCommand : INetworkCommand
{
    public int ServerId;
    public int SessionKey1;
    public int SessionKey2;

    public RequestServerLoginCommand(int serverId, int sessionKey1, int sessionKey2)
    {
        ServerId = serverId;
        SessionKey1 = sessionKey1;
        SessionKey2 = sessionKey2;
    }
}

