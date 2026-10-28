/// <summary>
/// Manor, quest list, and buffer UI access with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 38).
/// </summary>
public static class AuxiliaryGameUIPacketServices
{
    private static QuestListWindow _questList;
    private static SeedInfoWindow _seedInfo;
    private static SellCropListWindow _sellCrop;
    private static BufferPanel _buffer;

    public static void Bind(
        QuestListWindow questList = null,
        SeedInfoWindow seedInfo = null,
        SellCropListWindow sellCrop = null,
        BufferPanel buffer = null)
    {
        if (questList != null)
            _questList = questList;
        if (seedInfo != null)
            _seedInfo = seedInfo;
        if (sellCrop != null)
            _sellCrop = sellCrop;
        if (buffer != null)
            _buffer = buffer;
    }

    public static QuestListWindow QuestList =>
        _questList != null ? _questList : IncomingPacketActions.QuestList;

    public static SeedInfoWindow SeedInfo =>
        _seedInfo != null ? _seedInfo : IncomingPacketActions.SeedInfo;

    public static SellCropListWindow SellCrop =>
        _sellCrop != null ? _sellCrop : IncomingPacketActions.SellCrop;

    public static BufferPanel Buffer =>
        _buffer != null ? _buffer : IncomingPacketActions.Buffer;
}
