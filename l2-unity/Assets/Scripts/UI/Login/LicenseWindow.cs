using System;

public partial class LicenseWindow : L2Window
{
    [Inject] LoginRuntime _login;

    [Inject] IZoneTransport _zoneTransport;

    private IZoneTransport Transport =>
        _zoneTransport ?? (App.HasContainer ? App.Resolve<IZoneTransport>() : null);

    private static LicenseWindow _instance;
    [System.Obsolete("Use [Inject] or LoginPacketServices.LicenseWindow instead")]
    public static LicenseWindow Instance { get { return _instance; } }











}
