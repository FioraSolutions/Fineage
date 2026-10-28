/// <summary>
/// Shop preview info wire packet handler entry (#142 batch slice 148).
/// Re-homes unwired shop preview apply from <c>MerchantIncomingPacketServices</c> (sub-slice 80)
/// after shop list and warehouse facades split merchant apply paths (sub-slices 83–84).
/// </summary>
public static class ShopPreviewApplyIncomingPacketServices
{
    public static void ApplyShopPreviewInfo(ShopPreviewInfoDto packet)
    {
    }
}
