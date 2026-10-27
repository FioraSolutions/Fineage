using System.Threading;
using UnityEngine;

public class LoginClientPacketHandler : ClientPacketHandler
{
    readonly LoginClient _login;

    public LoginClientPacketHandler(LoginClient login)
    {
        _login = login;
    }

    LoginClient Login
    {
        get { return _login != null ? _login : IncomingPacketActions.Login; }
    }

    public override void SendPacket(IOutgoingPacket packet)
    {
        if (Login.LogSentPackets)
            Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] [LoginServer] Sending packet:" + (LoginClientPacketType)packet.GetPacketType());

        _client.SendPacket(packet);
    }
}
