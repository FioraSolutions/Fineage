using VContainer;

public static partial class TablePacketServices
{
    static ItemTable LegacyItemTableInstance() => ItemTable.Instance;
#pragma warning restore CS0618
}
