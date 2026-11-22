using UnityEditor;

[InitializeOnLoad]
static class PlayModeExitEditorHook
{
    static PlayModeExitEditorHook()
    {
        PlayModeExit.StopInEditor = () => EditorApplication.isPlaying = false;
    }
}
