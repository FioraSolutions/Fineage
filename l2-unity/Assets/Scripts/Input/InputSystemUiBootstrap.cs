using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Replaces legacy <see cref="StandaloneInputModule"/> (reads UnityEngine.Input) with
/// <see cref="InputSystemUIInputModule"/> so UI EventSystems work under Input System-only.
/// </summary>
public static class InputSystemUiBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Register()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AfterSceneLoad()
    {
        PatchAll();
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PatchAll();
    }

    static void PatchAll()
    {
        EventSystem[] systems = Object.FindObjectsByType<EventSystem>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        for (int i = 0; i < systems.Length; i++)
            Patch(systems[i]);
    }

    static void Patch(EventSystem eventSystem)
    {
        if (eventSystem == null)
            return;

        GameObject go = eventSystem.gameObject;
        StandaloneInputModule legacy = go.GetComponent<StandaloneInputModule>();
        InputSystemUIInputModule modern = go.GetComponent<InputSystemUIInputModule>();

        if (legacy != null)
        {
            // DestroyImmediate: StandaloneInputModule.UpdateModule reads Input.mousePosition
            // in the same frame if we only queue Destroy().
            Object.DestroyImmediate(legacy);
        }

        if (modern == null)
        {
            modern = go.AddComponent<InputSystemUIInputModule>();
            modern.AssignDefaultActions();
        }
    }
}
