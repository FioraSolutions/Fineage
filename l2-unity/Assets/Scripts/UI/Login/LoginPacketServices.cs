/// <summary>
/// Login UI access to services with [Inject] wiring and Networking/live fallback (#142).
/// </summary>
public static class LoginPacketServices
{
    private static LoginRuntime _login;

    public static void Bind(LoginRuntime login = null)
    {
        if (login != null)
            _login = login;
    }

    public static void ClearLoginRuntime()
    {
        _login = null;
    }

    public static LoginClient LoginClient =>
        _login != null ? _login.LoginClient : NetworkingPacketServices.Login;

    public static GameManager Manager
    {
        get
        {
            GameManager manager = _login != null ? _login.Manager : null;
            if (manager != null)
                return manager;
            return NetworkingPacketServices.Manager;
        }
    }

    public static LoginWindow LoginWindow =>
        _login != null ? _login.LoginWindow : NetworkingPacketServices.LoginWindow;

    public static LicenseWindow LicenseWindow =>
        _login != null ? _login.License : LicenseWindow.Instance;

    public static ServerSelectWindow ServerSelect =>
        _login != null ? _login.Servers : ServerSelectWindow.Instance;

    public static CharSelectWindow CharSelect =>
        _login != null ? _login.CharSelect : LegacyCharSelectWindowInstance();

    public static CharCreationWindow CharCreate =>
        _login != null ? _login.CharCreate : LegacyCharCreationWindowInstance();

    public static CharacterSelector Characters =>
        _login != null ? _login.Selector : CharacterSelector.Instance;

    public static CharacterCreator Creator =>
        _login != null ? _login.Creator : CharacterCreator.Instance;

    public static LoginCameraManager LoginCamera =>
        _login != null ? _login.Camera : LoginCameraManager.Instance;

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    static CharSelectWindow LegacyCharSelectWindowInstance() => CharSelectWindow.Instance;

    static CharCreationWindow LegacyCharCreationWindowInstance() => CharCreationWindow.Instance;
#pragma warning restore CS0618
}
