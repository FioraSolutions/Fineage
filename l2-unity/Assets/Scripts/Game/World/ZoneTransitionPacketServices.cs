/// <summary>DI/static access for <see cref="ZoneTransitionService"/> and banner overlay.</summary>
public static class ZoneTransitionPacketServices
{
    static ZoneTransitionService _service;
    static ZoneNameBannerOverlay _banner;

    public static void Bind(ZoneTransitionService service = null, ZoneNameBannerOverlay banner = null)
    {
        if (service != null)
            _service = service;
        if (banner != null)
            _banner = banner;
    }

    public static ZoneTransitionService Service => _service;
    public static ZoneNameBannerOverlay Banner => _banner ?? ZoneNameBannerOverlay.Instance;
}