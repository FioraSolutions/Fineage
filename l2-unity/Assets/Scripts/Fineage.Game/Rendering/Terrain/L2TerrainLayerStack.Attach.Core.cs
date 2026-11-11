using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public sealed partial class L2TerrainLayerStack
{
    public static void EnsureAttached()
    {
        if (HasOverlays)
            return;
        AttachAll();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AttachLoaded()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Camera.onPreCull -= OnPreCullAttach;
        Camera.onPreCull += OnPreCullAttach;
        AttachAll();
    }

    static void OnPreCullAttach(Camera camera)
    {
        if (camera == null || HasOverlays)
            return;
        AttachAll();
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AttachAll();
    }
}
