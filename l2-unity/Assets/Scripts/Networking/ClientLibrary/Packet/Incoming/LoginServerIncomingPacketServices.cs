/// <summary>
/// Login-server wire packet handler access via LoginPacketServices (#142 sub-slice 58).
/// </summary>
public static class LoginServerIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static GameClient Game => LoginPacketServices.Game;

    public static GameManager Manager => LoginPacketServices.Manager;

    public static LoginWindow LoginWindow => LoginPacketServices.LoginWindow;

    public static void OnInitParsed(InitDto packet)
    {
        Login.SetRSAKey(packet.PublicKey);
        Login.SetBlowFishKey(packet.BlowfishKey);
        Login.SetSessionId(packet.SessionId);
        Login.CompleteInitPacket();
        Login.Send(new AuthGameGuardCommand(packet.SessionId, packet.GG));
    }

    public static void OnGGAuthParsed(GGAuthDto packet)
    {
        LobbyFlowLog.Info("TX RequestAuthLogin account=" + (Login != null ? Login.Account : "?"));
        Login.Send(new RequestAuthLoginCommand(
            Login.Account,
            Login.Password,
            packet.Response));
    }

    public static void ApplyInit(InitDto packet)
    {
    }

    public static void ApplyGGAuth(GGAuthDto packet)
    {
    }

    public static void OnLoginOkParsed(LoginOkDto packet)
    {
        Login.SessionKey1 = packet.SessionKey1;
        Login.SessionKey2 = packet.SessionKey2;
        Game.SessionKey1 = packet.SessionKey1;
        Game.SessionKey2 = packet.SessionKey2;
    }

    public static void ApplyLoginOk(LoginOkDto packet)
    {
        Login.OnAuthAllowed();
    }

    public static void ApplyLoginFail(LoginFailDto packet)
    {
        LoginWindow.ShowErrorText(packet.Message);
        Login.Disconnect();
    }

    public static void OnPlayOkParsed(PlayOkDto packet)
    {
        Game.PlayKey1 = packet.PlayOk1;
        Game.PlayKey2 = packet.PlayOk2;
    }

    public static void ApplyPlayOk(PlayOkDto packet)
    {
        LobbyFlowLog.Info(
            "PlayOk Apply playKey1=" + packet.PlayOk1 + " playKey2=" + packet.PlayOk2 +
            " state=" + (Manager != null ? Manager.GameState.ToString() : "null"));

        Manager.OnLoginServerPlayOk();

        if (Manager.GameState != GameState.READY_TO_CONNECT)
        {
            LobbyFlowLog.Warn("PlayOk skip Game.Connect — GameState=" + Manager.GameState);
            return;
        }
        if (Manager.IsSwitchingServer)
        {
            LobbyFlowLog.Warn("PlayOk skip Game.Connect — already switching server");
            return;
        }

        Manager.IsSwitchingServer = true;
        Login.Disconnect();
        LobbyFlowLog.Info("PlayOk → Login.Disconnect + Game.Connect");
        Game.Connect();
    }

    public static void ApplyServerList(ServerListDto packet)
    {
        Login.OnServerListReceived(packet.LastServer, packet.ServersData, packet.CharsOnServers);
    }
}
