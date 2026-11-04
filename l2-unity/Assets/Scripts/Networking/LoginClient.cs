using UnityEngine;

/// <summary>
/// Pre-game connection facts: the account being signed in, and the RSA/Blowfish material the
/// Interlude login server hands out.
///
/// <para>
/// This used to be a <c>DefaultClient</c>: a Blowfish socket client with a send queue, a
/// receive thread, and a crypt key. Fineage Net owns the login — the lobby token is the zone
/// password — so the socket never opened. What survived is the data the packet layer writes
/// into it and the UI reads back.
/// </para>
///
/// <para>
/// The class stays because <c>IncomingPacketActions.Login</c> is frozen (#137) and names it.
/// </para>
/// </summary>
public class LoginClient : MonoBehaviour
{
    /// <summary>
    /// The Interlude static Blowfish key. Kept so <see cref="InitParsedApplyIncomingPacketServices"/>
    /// can mirror what the login server advertises; nothing encrypts with it on the Net wire.
    /// </summary>
    public static byte[] STATIC_BLOWFISH_KEY =
    {
        (byte)0x6b, (byte)0x60, (byte)0xcb, (byte)0x5b, (byte)0x82, (byte)0xce, (byte)0x90, (byte)0xb1,
        (byte)0xcc, (byte)0x2b, (byte)0x6c, (byte)0x55, (byte)0x6c, (byte)0x6c, (byte)0x6c, (byte)0x6c
    };

    [Header("Account")]
    [SerializeField] protected string _account;
    [SerializeField] protected string _password;

    private byte[] _rsaKey;
    private byte[] _blowfishKey;
    private int _sessionId;

    public string Account { get { return _account; } set { _account = value; } }
    public string Password { get { return _password; } set { _password = value; } }
    public byte[] RSAPublicKey { get { return _rsaKey; } }
    public byte[] BlowfishKey { get { return _blowfishKey; } }
    public int SessionId { get { return _sessionId; } }

    private static LoginClient _instance;
    public static LoginClient Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
    }

    // ---- Facts written by the login packet layer, read by the UI -------------------------

    public void SetRSAKey(byte[] rsaKey)
    {
        _rsaKey = rsaKey;
    }

    public void SetBlowFishKey(byte[] blowfishKey)
    {
        _blowfishKey = blowfishKey;
    }

    public void SetSessionId(int sessionId)
    {
        _sessionId = sessionId;
    }

    /// <summary>
    /// No-op kept for <see cref="InitParsedApplyIncomingPacketServices"/>: this used to flip a
    /// flag on the socket so the next received packet was not treated as a fragment. There is
    /// no socket to configure.
    /// </summary>
    public void CompleteInitPacket()
    {
    }
}
