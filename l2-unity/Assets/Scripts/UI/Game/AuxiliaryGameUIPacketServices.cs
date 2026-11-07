/// <summary>
/// Manor and buffer UI access with [Inject] wiring and live/peer fallback (#142).
/// </summary>
public static class AuxiliaryGameUIPacketServices
{
    private static SeedInfoWindow _seedInfo;
    private static SellCropListWindow _sellCrop;
    private static BufferPanel _buffer;

    public static void Bind(
        SeedInfoWindow seedInfo = null,
        SellCropListWindow sellCrop = null,
        BufferPanel buffer = null)
    {
        if (seedInfo != null)
            _seedInfo = seedInfo;
        if (sellCrop != null)
            _sellCrop = sellCrop;
        if (buffer != null)
            _buffer = buffer;
    }

    public static SeedInfoWindow SeedInfo =>
        _seedInfo != null ? _seedInfo : SeedInfoWindow.Instance;

    public static SellCropListWindow SellCrop =>
        _sellCrop != null ? _sellCrop : SellCropListWindow.Instance;

    public static BufferPanel Buffer =>
        _buffer != null ? _buffer : ControllerPacketServices.Buffer;
}
