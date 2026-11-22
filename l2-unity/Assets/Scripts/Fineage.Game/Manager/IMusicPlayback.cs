using UnityEngine;

/// <summary>
/// Background-music clear for <c>GameManager</c> restart (#131 S34d).
/// <c>MusicManager.Awake</c> binds the live player. There is no singleton fallback.
/// </summary>
public interface IMusicPlayback
{
    void Clear();
}

public static class MusicPlayback
{
    public static IMusicPlayback Current { get; private set; }

    public static void Bind(IMusicPlayback music)
    {
        if (music != null)
            Current = music;
    }

    public static void TryClear()
    {
        if (Current == null)
        {
            Debug.LogError("[MusicPlayback] IMusicPlayback missing — MusicManager.Awake did not bind, so background music was not cleared.");
            return;
        }

        Current.Clear();
    }
}
