/// <summary>
/// InterludeKey wire apply hook (#142 batch slice 146; login hub slice 176).
/// Splits apply out of <see cref="InterludeKeyIncomingPacketServices"/> (sub-slices 53 / 76).
/// </summary>
public static class InterludeKeyApplyIncomingPacketServices
{
    public static LoginClient Login => LoginSessionApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginSessionApplyIncomingPacketServices.Game;

    public static void ApplyInterludeKey(InterludeKeyDto packet)
    {
        if (!packet.AuthAllowed)
        {
            LobbyFlowLog.Warn("InterludeKey AuthAllowed=false → disconnect");
            Game.Disconnect();
            Login.Disconnect();
            return;
        }

        var login = Login;
        var game = Game;
        if (login == null || game == null)
        {
            LobbyFlowLog.Warn("InterludeKey AuthLogin skipped — Login/Game null");
            return;
        }

        // Direct wire write (bypass SendGame queue): evidence showed TX AuthLogin logged
        // while stub never got RX AuthLogin — queue/handler races after PlayOk socket replace.
        LobbyFlowLog.Info(
            "TX AuthLogin account=" + login.Account +
            " play=" + game.PlayKey1.ToString("X8") + "/" + game.PlayKey2.ToString("X8") +
            " login=" + game.SessionKey1.ToString("X8") + "/" + game.SessionKey2.ToString("X8") +
            " cryptEnabled=" + game.IsCryptEnabled());
        bool wrote = game.SendAuthLoginNow(
            login.Account,
            game.PlayKey1,
            game.PlayKey2,
            game.SessionKey1,
            game.SessionKey2);
        if (!wrote)
            LobbyFlowLog.Error("AuthLogin WIRE FAIL — socket write skipped/failed (stub will hang after InterludeKey)");
        else
            LobbyFlowLog.Info("AuthLogin WIRE OK — waiting CharSelectionInfo");
    }
}
