using System.Collections.Generic;
using UnityEngine;

public sealed partial class MoveToCommand : INetworkCommand
{
    public ValidatePositionCommand(float x, float y, float z, int heading)
    {
        X = x;
        Y = y;
        Z = z;
        Heading = heading;
    }

    public RequestMagicSkillUseCommand(int skillId, int ctrlPressed, byte shiftPressed)
    {
        SkillId = skillId;
        CtrlPressed = ctrlPressed;
        ShiftPressed = shiftPressed;
    }

    public RequestSay2Command(ChatTypeData data, string message, string targetName)
    {
        Data = data;
        Message = message;
        TargetName = targetName;
    }

    public RequestBypassToServerCommand(string bypass)
    {
        Bypass = bypass;
    }

    public RequestUserCommandCommand(int id)
    {
        Id = id;
    }

    public RequestDestroyItemCommand(int objectId, int count)
    {
        ObjectId = objectId;
        Count = count;
    }

    public RequestDropItemCommand(int objectId, int count, int x, int y, int z)
    {
        ObjectId = objectId;
        Count = count;
        X = x;
        Y = y;
        Z = z;
    }

    public RequestEnchantItemCommand(int objectId)
    {
        ObjectId = objectId;
    }

    public RequestQuestAbortCommand(int questId)
    {
        QuestId = questId;
    }
}
