public static partial class NetPaperdollApply
{
    public static void ApplyItemIds(PlayerAppearance appearance, int[] itemIds)
    {
        if (appearance == null || itemIds == null)
            return;

        PaperDollSelection doll = appearance.PaperDoll;
        doll.Item_Under = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Under);
        doll.Item_Lear = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.LeftEar);
        doll.Item_Rear = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.RightEar);
        doll.Item_Neck = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Neck);
        doll.Item_LFinger = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.LeftFinger);
        doll.Item_RFinger = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.RightFinger);
        doll.Item_Head = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Head);
        doll.Item_RHand = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.RightHand);
        doll.Item_LHand = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.LeftHand);
        doll.Item_Gloves = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Gloves);
        doll.Item_Chest = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Chest);
        doll.Item_Legs = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Legs);
        doll.Item_Feet = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Feet);
        doll.Item_Cloak = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Cloak);
        doll.Item_Face = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Face);
        doll.Item_Hair = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Hair);
        doll.Item_Hair2 = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Hair2);
        appearance.RHand = doll.Item_RHand;
        appearance.LHand = doll.Item_LHand;
        appearance.Gloves = doll.Item_Gloves;
        appearance.Chest = doll.Item_Chest;
        appearance.Legs = doll.Item_Legs;
        appearance.Feet = doll.Item_Feet;
        appearance.PaperdollReady = true;
    }

    static void ApplyObjectIds(PaperDollSelection doll, NetPaperdollSlot[] slots)
    {
        if (doll == null || slots == null)
            return;
        for (int i = 0; i < slots.Length; i++)
        {
            int objectId = slots[i].ObjectId;
            switch (i)
            {
                case NetPaperdoll.Under:
                    doll.Obj_Under = objectId;
                    break;
                case NetPaperdoll.LeftEar:
                    doll.Obj_Lear = objectId;
                    break;
                case NetPaperdoll.RightEar:
                    doll.Obj_Pear = objectId;
                    break;
                case NetPaperdoll.Neck:
                    doll.Obj_Neck = objectId;
                    break;
                case NetPaperdoll.LeftFinger:
                    doll.Obj_LFinger = objectId;
                    break;
                case NetPaperdoll.RightFinger:
                    doll.Obj_RFinger = objectId;
                    break;
                case NetPaperdoll.Head:
                    doll.Obj_Head = objectId;
                    break;
                case NetPaperdoll.RightHand:
                    doll.Obj_RHand = objectId;
                    break;
                case NetPaperdoll.LeftHand:
                    doll.Obj_LHand = objectId;
                    break;
                case NetPaperdoll.Gloves:
                    doll.Obj_Gloves = objectId;
                    break;
                case NetPaperdoll.Chest:
                    doll.Obj_Chest = objectId;
                    break;
                case NetPaperdoll.Legs:
                    doll.Obj_Legs = objectId;
                    break;
                case NetPaperdoll.Feet:
                    doll.Obj_Feet = objectId;
                    break;
                case NetPaperdoll.Cloak:
                    doll.Obj_Cloak = objectId;
                    break;
                case NetPaperdoll.Face:
                    doll.Obj_Face = objectId;
                    break;
                case NetPaperdoll.Hair:
                    doll.Obj_Hair = objectId;
                    break;
                case NetPaperdoll.Hair2:
                    doll.Obj_Hair2 = objectId;
                    break;
            }
        }
    }
}
