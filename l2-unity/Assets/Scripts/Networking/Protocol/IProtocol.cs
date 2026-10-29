/// One implementation per chronicle. Commands in, bytes out. Raw bytes in, models out.
public interface IProtocol
{
    IOutgoingPacket EncodeGame(INetworkCommand command, bool encrypt);

    bool TryParseGame(byte[] raw, bool cryptEnabled, out INetworkModel model);

    void SetGameCryptKey(byte[] key);
}
