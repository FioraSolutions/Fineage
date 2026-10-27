/// <summary>
/// App and login lifetime wiring for incoming handler services (#142 sub-slice 54).
/// </summary>
public static class AppLoginIncomingPacketServices
{
    public static void BindApp(
        GameClient game,
        LoginClient login,
        GameManager manager,
        PacketApplyQueue applyQueue,
        EventProcessor events)
    {
        DiRuntimePacketServices.Bind(game: game, login: login, manager: manager);
        NetworkingPacketServices.Bind(manager: manager, login: login, events: events);
        IncomingPacketServices.Bind(
            game: game,
            login: login,
            manager: manager,
            applyQueue: applyQueue,
            events: events);
    }

    public static void BindLogin(LoginRuntime loginRuntime)
    {
        if (loginRuntime == null)
            return;

        LoginPacketServices.Bind(login: loginRuntime);
        IncomingPacketServices.Bind(
            game: loginRuntime.Game,
            login: loginRuntime.LoginClient,
            manager: loginRuntime.Manager,
            loginWindow: loginRuntime.LoginWindow,
            charSelect: loginRuntime.CharSelect,
            characters: loginRuntime.Selector);
    }
}
