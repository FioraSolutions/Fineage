using UnityEngine;

/// <summary>
/// Zone-session facts that outlived the Interlude game socket: the server row the player
/// picked, the PlayOk keys, and whether world data has finished loading.
///
/// <para>
/// This used to be a <c>DefaultClient</c> — an <see cref="AsynchronousClient"/> with its own
/// send queue, Blowfish crypt key, and PlayOk → Connect handshake. Fineage Net owns the only
/// zone socket (<c>NetSession</c>), so none of that ran. Commands go through
/// <see cref="ZoneCommands"/> and profile state lives on <see cref="IZoneTransport"/>; what is
/// left here is the handful of fields the packet layer still assigns.
/// </para>
///
/// <para>
/// The class stays because <c>IncomingPacketActions.Game</c> is frozen (#137) and names it.
/// </para>
/// </summary>
public class GameClient : MonoBehaviour
{
    private int _serverId;
    private int _playKey1;
    private int _playKey2;

    public int ServerId { get { return _serverId; } set { _serverId = value; } }
    public int PlayKey1 { get { return _playKey1; } set { _playKey1 = value; } }
    public int PlayKey2 { get { return _playKey2; } set { _playKey2 = value; } }

    private static GameClient _instance;
    public static GameClient Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
    }

    /// <summary>
    /// Ask the zone for the remaining skill cooldowns after a world load.
    ///
    /// <para>
    /// This lived on the socket client because that is where <c>Send</c> used to be. It is a
    /// plain zone command, so it goes through the same path as everything else.
    /// </para>
    /// </summary>
    public void EndLoadWorld()
    {
        ZoneCommands.Send(new RequestSkillCoolTimeCommand());
    }
}
