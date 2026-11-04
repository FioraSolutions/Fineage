using System;

/// <summary>
/// PlayOk wire apply hook (#142 batch slice 146; login hub relay slice 257).
/// Splits apply out of <see cref="PlayOkIncomingPacketServices"/> (sub-slice 58).
/// </summary>
public static class PlayOkApplyIncomingPacketServices
{
    public static LoginClient Login => LoginSessionApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginSessionApplyIncomingPacketServices.Game;

    public static GameManager Manager => LoginSessionApplyIncomingPacketServices.Manager;

    public static void Queue(Action action) => LoginSessionApplyIncomingPacketServices.Queue(action);

    public static void ApplyPlayOk(PlayOkDto packet) =>
        Queue(() => ApplyPlayOkCore(packet));

    static void ApplyPlayOkCore(PlayOkDto packet)
    {
        LobbyFlowLog.Info(
            "PlayOk Apply playKey1=" + packet.PlayOk1 + " playKey2=" + packet.PlayOk2 +
            " state=" + (Manager != null ? Manager.GameState.ToString() : "null"));

        Manager.OnLoginServerPlayOk();

        // This is the Interlude login-server → game-socket handoff: PlayOk carries the
        // Blowfish key, then Connect() opens a second socket and sends AuthLogin on it.
        // Fineage Net has no such handoff -- the lobby token is the zone password and
        // NetSession owns the only socket -- so there is nothing left to connect to.
        // Bridge the state so a stray PlayOk cannot wedge the lobby.
        if (Manager.GameState == GameState.READY_TO_CONNECT)
        {
            LobbyFlowLog.Info("PlayOk on Net wire — no Interlude game socket to connect, skipping handoff");
            Manager.IsSwitchingServer = false;
        }
    }
}
