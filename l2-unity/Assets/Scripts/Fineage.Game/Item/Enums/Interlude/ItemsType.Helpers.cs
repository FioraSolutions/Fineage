public partial class ItemsType
{
    public static ItemSlot ParceSlot(int slot)
    {
        switch (slot)
        {
            case 8:
                return ItemSlot.neck;
            case 64:
                return ItemSlot.head;
            case 100:
                return ItemSlot.lhand;
            case 512:
                return ItemSlot.gloves;
            case 1024:
                return ItemSlot.chest;
            case 128:
                return ItemSlot.rhand;
            case 6:
                return ItemSlot.lear;
            case 1000:
                return ItemSlot.feet;
            case 2048:
                return ItemSlot.legs;
            case 4096:
                return ItemSlot.boots;
            case 48:
                return ItemSlot.lfinger;
            case 256:
                return ItemSlot.lhand;
            case 16384:
                return ItemSlot.lrhand;
            case 32768:
                return ItemSlot.fullarmor;

            default:
                return ItemSlot.none;
        }
    }
}
