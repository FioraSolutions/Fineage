/// <summary>
/// InterludeKey / AuthLogin wire packet handler entry (#142 sub-slice 106).
/// Splits interlude-key parse/apply out of <see cref="LobbyWireIncomingPacketServices"/> (sub-slices 53 / 76 encapsulation).
/// </summary>
public static class InterludeKeyIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static GameClient Game => LoginPacketServices.Game;

    public static void OnInterludeKeyParsed(InterludeKeyDto packet)
    {
        // Handler instance is a registry singleton — never gate AuthLogin on sticky fields
        // (that skipped AuthLogin after ServerList Confirm → PlayOk on every reconnect).
        if (!packet.AuthAllowed)
            return;

        // Crypt must arm on the parse thread before later encrypted RX; AuthLogin itself
        // is sent from Apply (main/apply queue) so it cannot race SendGame.Stop/replace.
        if (!packet.UseBlowfish)
            return;

        byte[] equalsKey = BlowFishStaticKey.GetCreateFullKeyBlowFish(packet.BlowFishKey);
        Game.EnableCrypt(equalsKey);
    }

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
