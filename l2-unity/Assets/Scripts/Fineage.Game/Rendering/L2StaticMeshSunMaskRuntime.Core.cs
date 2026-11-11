using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public sealed partial class L2StaticMeshSunMaskRuntime : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (_installed)
            return;
        _installed = true;
        if (!LoadSunMasks)
        {
            // Debug.Log("[L2StaticMeshSunMask] load paused");
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        for (int i = 0; i < SceneManager.sceneCount; i++)
            ApplyScene(SceneManager.GetSceneAt(i));
    }
}
