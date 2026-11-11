using System.Collections.Generic;
using UnityEngine;

public sealed partial class MoveToCommand : INetworkCommand
{
    public MoveToCommand(Vector3 from, Vector3 to)
    {
        From = from;
        To = to;
    }

    public UseItemCommand(int objectId, int ctrlPressed)
    {
        ObjectId = objectId;
        CtrlPressed = ctrlPressed;
    }

    public ZoneEnchantRequestCommand(int scrollObjectId)
    {
        ScrollObjectId = scrollObjectId;
    }

    public UnequipItemCommand(int paperdollSlot)
    {
        PaperdollSlot = paperdollSlot;
    }

    public ZoneGrantItemCommand(int itemId, int count = 1)
    {
        ItemId = itemId;
        Count = count < 1 ? 1 : count;
    }

    public ZonePickupRequestCommand(int groundObjectId)
    {
        GroundObjectId = groundObjectId;
    }

    public RequestAutoSoulShotCommand(int itemId, bool enabled)
    {
        ItemId = itemId;
        Enabled = enabled;
    }

    public ClickActionCommand(int objectId, int originX, int originY, int originZ, int actionId)
    {
        ObjectId = objectId;
        OriginX = originX;
        OriginY = originY;
        OriginZ = originZ;
        ActionId = actionId;
    }
}
