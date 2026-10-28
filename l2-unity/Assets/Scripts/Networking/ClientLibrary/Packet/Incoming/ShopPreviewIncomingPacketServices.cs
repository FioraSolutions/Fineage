/// <summary>
/// Shop preview info wire packet handler entry (#142 sub-slice 98).
/// Re-homes unwired shop preview apply from <c>MerchantIncomingPacketServices</c> (sub-slice 80)
/// after shop list and warehouse facades split merchant apply paths (sub-slices 83–84).
/// </summary>
public static class ShopPreviewIncomingPacketServices
{
    public static void ApplyShopPreviewInfo(ShopPreviewInfoDto packet)
    {
    }
}
