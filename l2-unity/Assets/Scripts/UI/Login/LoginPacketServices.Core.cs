public static partial class LoginPacketServices
{
    public static void Bind(LoginRuntime login = null)
    {
        if (login != null)
            _login = login;
    }

    public static void ClearLoginRuntime()
    {
        _login = null;
    }

    static L2LoginUI LegacyL2LoginUiInstance() => L2LoginUI.Instance;

    static LicenseWindow LegacyLicenseWindowInstance() => LicenseWindow.Instance;

    static ServerSelectWindow LegacyServerSelectWindowInstance() => ServerSelectWindow.Instance;
}
