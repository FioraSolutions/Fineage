public static partial class NetPaperdollApply
{
    public static void ApplyUserPaperdoll(PlayerAppearance appearance, NetPaperdollSlot[] slots)
    {
        if (appearance == null || slots == null)
            return;
        ApplyObjectIds(appearance.PaperDoll, slots);
        ApplyItemIds(appearance, NetPaperdoll.ItemIds(slots));
    }
}
