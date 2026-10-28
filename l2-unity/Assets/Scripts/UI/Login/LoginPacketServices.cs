/// <summary>
/// Login UI access to services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 11).
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
        IncomingPacketActions.BindLogin(null);
    }

    public static LoginClient LoginClient =>
        _login != null ? _login.LoginClient : IncomingPacketActions.Login;

    public static GameClient Game =>
        _login != null ? _login.Game : IncomingPacketActions.Game;

    public static GameManager Manager
    {
        get
        {
            GameManager manager = _login != null ? _login.Manager : IncomingPacketActions.Manager;
            if (manager != null)
                return manager;
            return NetworkingPacketServices.Manager;
        }
    }

    public static LoginWindow LoginWindow =>
        _login != null ? _login.LoginWindow : IncomingPacketActions.LoginWindow;

    public static LicenseWindow LicenseWindow =>
        _login != null ? _login.License : IncomingPacketActions.LicenseWindow;

    public static ServerSelectWindow ServerSelect =>
        _login != null ? _login.Servers : IncomingPacketActions.ServerSelect;

    public static CharSelectWindow CharSelect =>
        _login != null ? _login.CharSelect : IncomingPacketActions.CharSelect;

    public static CharCreationWindow CharCreate =>
        _login != null ? _login.CharCreate : IncomingPacketActions.CharCreate;

    public static CharacterSelector Characters =>
        _login != null ? _login.Selector : IncomingPacketActions.Characters;

    public static CharacterCreator Creator =>
        _login != null ? _login.Creator : IncomingPacketActions.Creator;

    public static LoginCameraManager LoginCamera =>
        _login != null ? _login.Camera : IncomingPacketActions.LoginCamera;
}
