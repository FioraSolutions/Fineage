using UnityEngine;

public static partial class LoginPacketServices
{
    private static LoginRuntime _login;



    public static LoginClient LoginClient =>
        _login != null ? _login.LoginClient : NetworkingPacketServices.Login;

    public static GameManager Manager
    {
        get
        {
            GameManager manager = _login != null ? _login.Manager : null;
            if (manager != null)
                return manager;
            // Prefer App-bound NetGameHost before NetworkingPacketServices (A09 residual).
            if (NetGameHost.TryGet() is GameManager hosted)
                return hosted;
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







#pragma warning restore CS0618
}
