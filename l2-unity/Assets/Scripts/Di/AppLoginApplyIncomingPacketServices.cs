/// <summary>
/// App and login lifetime wiring for incoming handler services (#142 sub-slice 54; bind facade slice 174–175).
/// </summary>
public static class AppLoginSessionApplyIncomingPacketServices
{
    public static void BindApp(
        GameClient game,
        GameManager manager,
        PacketApplyQueue applyQueue,
        EventProcessor events)
    {
        DiRuntimePacketServices.Bind(game: game, manager: manager);
        NetworkingPacketServices.Bind(manager: manager, events: events);
        AppLoginBindApplyIncomingPacketServices.BindApp(
            game: game,
            manager: manager,
            applyQueue: applyQueue,
            events: events);
    }

    public static void BindLogin(LoginRuntime loginRuntime)
    {
        if (loginRuntime == null)
            return;

        LoginPacketServices.Bind(login: loginRuntime);
        AppLoginBindApplyIncomingPacketServices.BindLogin(loginRuntime);
    }
}
