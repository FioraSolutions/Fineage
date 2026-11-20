using UnityEngine;

public static partial class InterludeSfx
{
    public static AudioClip Load(string resourcesPath) =>
        InterludeSfxBridge.LoadClip != null ? InterludeSfxBridge.LoadClip(resourcesPath) : null;
}