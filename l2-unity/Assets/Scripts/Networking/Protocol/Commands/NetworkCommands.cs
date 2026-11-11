using System.Collections.Generic;
using UnityEngine;

public sealed class MoveToCommand : INetworkCommand
{
    public Vector3 From;
    public Vector3 To;

    public MoveToCommand(Vector3 from, Vector3 to)
    {
        From = from;
        To = to;
    }
}

public sealed class UseItemCommand : INetworkCommand
{
    public int ObjectId;
    public int CtrlPressed;

    public UseItemCommand(int objectId, int ctrlPressed)
    {
        ObjectId = objectId;
        CtrlPressed = ctrlPressed;
    }
}

/// <summary>Fineage Net TX opcode 39 — select an enchant scroll and open the window.</summary>

/// <summary>Fineage Net TX opcode 39 — select an enchant scroll and open the window.</summary>
public sealed class ZoneEnchantRequestCommand : INetworkCommand
{
    public int ScrollObjectId;

    public ZoneEnchantRequestCommand(int scrollObjectId)
    {
        ScrollObjectId = scrollObjectId;
    }
}

/// <summary>Fineage Net TX opcode 16 — clear paperdoll index 0..16.</summary>

/// <summary>Fineage Net TX opcode 16 — clear paperdoll index 0..16.</summary>
public sealed class UnequipItemCommand : INetworkCommand
{
    public int PaperdollSlot;

    public UnequipItemCommand(int paperdollSlot)
    {
        PaperdollSlot = paperdollSlot;
    }
}

/// <summary>Fineage Net TX opcode 13 — server grants into bag via InventoryUpdate (Backend #120).</summary>

/// <summary>Fineage Net TX opcode 13 — server grants into bag via InventoryUpdate (Backend #120).</summary>
public sealed class ZoneGrantItemCommand : INetworkCommand
{
    public int ItemId;
    public int Count;

    public ZoneGrantItemCommand(int itemId, int count = 1)
    {
        ItemId = itemId;
        Count = count < 1 ? 1 : count;
    }
}

/// <summary>Fineage Net TX opcode 14 — pick up ground item (Backend C2 / #125).</summary>

/// <summary>Fineage Net TX opcode 14 — pick up ground item (Backend C2 / #125).</summary>
public sealed class ZonePickupRequestCommand : INetworkCommand
{
    public int GroundObjectId;

    public ZonePickupRequestCommand(int groundObjectId)
    {
        GroundObjectId = groundObjectId;
    }
}

/// <summary>
/// Mobius <c>RequestAutoSoulShot</c>: toggle automatic shot use for a carried shot item.
/// The zone answers with <c>AutoSoulShot</c> and then recharges the weapon after every swing.
/// </summary>

/// <summary>
/// Mobius <c>RequestAutoSoulShot</c>: toggle automatic shot use for a carried shot item.
/// The zone answers with <c>AutoSoulShot</c> and then recharges the weapon after every swing.
/// </summary>
public sealed class RequestAutoSoulShotCommand : INetworkCommand
{
    public int ItemId;
    public bool Enabled;

    public RequestAutoSoulShotCommand(int itemId, bool enabled)
    {
        ItemId = itemId;
        Enabled = enabled;
    }
}

public sealed class ClickActionCommand : INetworkCommand
{
    public int ObjectId;
    public int OriginX;
    public int OriginY;
    public int OriginZ;
    public int ActionId;

    public ClickActionCommand(int objectId, int originX, int originY, int originZ, int actionId)
    {
        ObjectId = objectId;
        OriginX = originX;
        OriginY = originY;
        OriginZ = originZ;
        ActionId = actionId;
    }
}

public sealed class ValidatePositionCommand : INetworkCommand
{
    public float X;
    public float Y;
    public float Z;
    public int Heading;

    public ValidatePositionCommand(float x, float y, float z, int heading)
    {
        X = x;
        Y = y;
        Z = z;
        Heading = heading;
    }
}

public sealed class RequestSkillCoolTimeCommand : INetworkCommand
{
}

public sealed class RequestTargetCanceldCommand : INetworkCommand
{
}

public sealed class RequestMagicSkillUseCommand : INetworkCommand
{
    public int SkillId;
    public int CtrlPressed;
    public byte ShiftPressed;

    public RequestMagicSkillUseCommand(int skillId, int ctrlPressed, byte shiftPressed)
    {
        SkillId = skillId;
        CtrlPressed = ctrlPressed;
        ShiftPressed = shiftPressed;
    }
}

public sealed class RequestSay2Command : INetworkCommand
{
    public ChatTypeData Data;
    public string Message;
    public string TargetName;

    public RequestSay2Command(ChatTypeData data, string message, string targetName)
    {
        Data = data;
        Message = message;
        TargetName = targetName;
    }
}

public sealed class RequestBypassToServerCommand : INetworkCommand
{
    public string Bypass;

    public RequestBypassToServerCommand(string bypass)
    {
        Bypass = bypass;
    }
}

public sealed class RequestUserCommandCommand : INetworkCommand
{
    public int Id;

    public RequestUserCommandCommand(int id)
    {
        Id = id;
    }
}

public sealed class RequestDestroyItemCommand : INetworkCommand
{
    public int ObjectId;
    public int Count;

    public RequestDestroyItemCommand(int objectId, int count)
    {
        ObjectId = objectId;
        Count = count;
    }
}

public sealed class RequestDropItemCommand : INetworkCommand
{
    public int ObjectId;
    public int Count;
    public int X;
    public int Y;
    public int Z;

    public RequestDropItemCommand(int objectId, int count, int x, int y, int z)
    {
        ObjectId = objectId;
        Count = count;
        X = x;
        Y = y;
        Z = z;
    }
}

public sealed class RequestEnchantItemCommand : INetworkCommand
{
    public int ObjectId;

    public RequestEnchantItemCommand(int objectId)
    {
        ObjectId = objectId;
    }
}

public sealed class RequestQuestListCommand : INetworkCommand
{
}

public sealed class RequestQuestAbortCommand : INetworkCommand
{
    public int QuestId;

    public RequestQuestAbortCommand(int questId)
    {
        QuestId = questId;
    }
}

/// <summary>Fineage Net TX opcode 32. Body key 0 is the clan name.</summary>

