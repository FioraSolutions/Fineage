using L2_login;
using UnityEngine;

public sealed class InterludeProtocol : IProtocol
{
    private readonly IIncomingPacketAutoRegistry _packets;
    private readonly IOutgoingPacketAutoRegistry _outgoing;
    private readonly object _gameCryptLock = new object();
    private GameCrypt _gameCrypt;

    public InterludeProtocol(IIncomingPacketAutoRegistry packets, IOutgoingPacketAutoRegistry outgoing)
    {
        _packets = packets;
        _outgoing = outgoing;
    }

    public IOutgoingPacket EncodeGame(INetworkCommand command, bool encrypt)
    {
        IOutgoingPacket packet = ToPacket(command);
        if (packet == null)
            return null;

        if (encrypt)
        {
            byte[] data = packet.GetData();
            lock (_gameCryptLock)
            {
                if (_gameCrypt != null)
                    _gameCrypt.Encrypt(data, 0, data.Length);
            }
        }

        return packet;
    }

    public bool TryParseGame(byte[] raw, bool cryptEnabled, out INetworkModel model)
    {
        model = null;
        if (raw == null || raw.Length == 0)
            return false;

        if (cryptEnabled)
        {
            lock (_gameCryptLock)
            {
                if (_gameCrypt != null)
                    _gameCrypt.Decrypt(raw, 0, raw.Length);
            }
        }

        byte opcode = raw[0];
        if (!_packets.IsGameOpcode(opcode))
        {
            // LobbyFlowLog.Warn(
                // "unknown game opcode 0x" + opcode.ToString("X2") +
                // " name=" + LobbyFlowLog.OpcodeName(opcode) +
                // " len=" + raw.Length + " crypt=" + cryptEnabled);
            return false;
        }

        bool parsed = _packets.TryParseGame(new ItemServer(raw), out model);
        if (!parsed)
        {
            // LobbyFlowLog.Warn(
                // "TryParseGame returned false opcode=0x" + opcode.ToString("X2") +
                // " " + LobbyFlowLog.OpcodeName(opcode) + " len=" + raw.Length);
        }
        else
        {
            // LobbyFlowLog.Info(
                // "parsed opcode=0x" + opcode.ToString("X2") + " " + LobbyFlowLog.OpcodeName(opcode) +
                // " dto=" + model.GetType().Name + " len=" + raw.Length);
        }

        return parsed;
    }

    public void SetGameCryptKey(byte[] key)
    {
        lock (_gameCryptLock)
        {
            _gameCrypt = new GameCrypt();
            _gameCrypt.SetKey(key);
        }
    }

    private IOutgoingPacket ToPacket(INetworkCommand command)
    {
        return _outgoing.Create(command);
    }
}
