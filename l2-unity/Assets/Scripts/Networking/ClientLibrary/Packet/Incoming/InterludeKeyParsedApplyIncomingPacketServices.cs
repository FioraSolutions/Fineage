/// <summary>
/// InterludeKey wire parse hook (#142 batch slice 146).
/// Splits parse out of <see cref="InterludeKeyIncomingPacketServices"/> (sub-slices 53 / 76).
/// </summary>
public static class InterludeKeyParsedApplyIncomingPacketServices
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
}
