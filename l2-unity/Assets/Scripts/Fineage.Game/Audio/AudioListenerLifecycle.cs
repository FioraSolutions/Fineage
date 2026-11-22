using UnityEngine;

/// <summary>
/// One AudioListener, on the active main camera. Playback does not call
/// AudioSource.Play while that camera is absent, so scene loads do not emit
/// "There are no audio listeners in the scene".
/// </summary>
public static class AudioListenerLifecycle
{
    static int _silencedFrame = -1;

    public static bool TryPrepare()
    {
        Camera camera = Camera.main;
        if (camera == null || !camera.isActiveAndEnabled)
            return false;
        return Attach(camera);
    }

    public static bool Attach(Camera camera)
    {
        if (camera == null)
            return false;

        AudioListener listener = camera.GetComponent<AudioListener>();
        if (listener == null)
            listener = camera.gameObject.AddComponent<AudioListener>();
        listener.enabled = true;
        if (!camera.isActiveAndEnabled || !listener.isActiveAndEnabled)
            return false;

        if (_silencedFrame != Time.frameCount)
        {
            _silencedFrame = Time.frameCount;
            SilenceOthers(listener);
        }

        return true;
    }

    static void SilenceOthers(AudioListener keep)
    {
        AudioListener[] all = Object.FindObjectsByType<AudioListener>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            AudioListener other = all[i];
            if (other == null || other == keep)
                continue;
            other.enabled = false;
        }
    }
}
