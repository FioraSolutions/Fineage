using UnityEngine;

/// <summary>
/// Remnant of the Interlude game socket; kept as a MonoBehaviour shell for DI / scene wiring.
///
/// <para>
/// It used to be a socket client with its own send queue, Blowfish crypt, and PlayOk
/// handshake. Fineage Net owns the only zone socket (<c>NetSession</c>), so every field here
/// ended up written by nobody and read by nobody.
/// Commands go through <see cref="ZoneCommands"/> and profile state lives on
/// <see cref="IZoneTransport"/>; the frozen accessor just needs a non-null instance.
/// </para>
/// </summary>
public class GameClient : MonoBehaviour
{
    private static GameClient _instance;

    public static GameClient Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
    }
}
