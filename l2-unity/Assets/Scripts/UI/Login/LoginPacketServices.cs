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

    public static L2LoginUI Ui =>
        (_login != null ? _login.Ui : null) ?? LegacyL2LoginUiInstance();

    public static LoginWindow LoginWindow =>
        (_login != null ? _login.LoginWindow : null) ?? NetworkingPacketServices.LoginWindow;

    public static LicenseWindow LicenseWindow =>
        (_login != null ? _login.License : null) ?? LegacyLicenseWindowInstance();

    public static ServerSelectWindow ServerSelect =>
        (_login != null ? _login.Servers : null) ?? LegacyServerSelectWindowInstance();

    public static CharSelectWindow CharSelect =>
        (_login != null ? _login.CharSelect : null) ?? LegacyCharSelectWindowInstance();

    public static CharCreationWindow CharCreate =>
        (_login != null ? _login.CharCreate : null) ?? LegacyCharCreationWindowInstance();

    public static CharacterSelector Characters =>
        (_login != null ? _login.Selector : null) ?? LegacyCharacterSelectorInstance();

    public static CharacterCreator Creator =>
        (_login != null ? _login.Creator : null) ?? LegacyCharacterCreatorInstance();

    public static LoginCameraManager LoginCamera =>
        (_login != null ? _login.Camera : null) ?? LegacyLoginCameraInstance();

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    static L2LoginUI LegacyL2LoginUiInstance() => L2LoginUI.Instance;

    static LicenseWindow LegacyLicenseWindowInstance() => LicenseWindow.Instance;

    static ServerSelectWindow LegacyServerSelectWindowInstance() => ServerSelectWindow.Instance;

    static CharSelectWindow LegacyCharSelectWindowInstance() => CharSelectWindow.Instance;

    static CharCreationWindow LegacyCharCreationWindowInstance() => CharCreationWindow.Instance;

    static CharacterSelector LegacyCharacterSelectorInstance() => CharacterSelector.Instance;

    static CharacterCreator LegacyCharacterCreatorInstance() => CharacterCreator.Instance;

    static LoginCameraManager LegacyLoginCameraInstance() => LoginCameraManager.Instance;
#pragma warning restore CS0618
}
