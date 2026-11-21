using UnityEngine;

public static class FineagePerfOverlayBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InstallOverlay()
    {
        if (!FineagePerfOverlaySettings.Enabled)
            return;

        if (Object.FindAnyObjectByType<FineageFrameTimingOverlay>() != null)
            return;

        var go = new GameObject(nameof(FineageFrameTimingOverlay));
        go.hideFlags = HideFlags.HideAndDontSave;
        go.AddComponent<FineageFrameTimingOverlay>();
    }
}