using System;
using UnityEngine;

/// <summary>
/// Leave play mode without a runtime reference to <c>UnityEditor</c> (#131 S34c).
/// The editor hook assigns <see cref="StopInEditor"/> on load.
/// </summary>
public static class PlayModeExit
{
    public static Action StopInEditor;

    public static void Stop()
    {
        if (Application.isEditor)
        {
            if (StopInEditor == null)
            {
                Debug.LogError("[PlayModeExit] StopInEditor was not set — PlayModeExitEditorHook did not register, so play mode was not stopped.");
                return;
            }

            StopInEditor();
            return;
        }

        Application.Quit();
    }
}
