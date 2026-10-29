/// <summary>
/// App/login incoming handler DI bind wiring (#142 batch slice 175).
/// </summary>
public static class AppLoginBindApplyIncomingPacketServices
{
    public static void BindApp(
        GameClient game,
        GameManager manager,
        PacketApplyQueue applyQueue,
        EventProcessor events) =>
        BindApplyIncomingPacketServices.Bind(
            game: game,
            manager: manager,
            applyQueue: applyQueue,
            events: events);

    public static void BindLogin(LoginRuntime loginRuntime)
    {
        if (loginRuntime == null)
            return;

        BindApplyIncomingPacketServices.Bind(
            game: loginRuntime.Game,
            manager: loginRuntime.Manager,
            loginWindow: loginRuntime.LoginWindow,
            charSelect: loginRuntime.CharSelect,
            characters: loginRuntime.Selector);
    }
}
