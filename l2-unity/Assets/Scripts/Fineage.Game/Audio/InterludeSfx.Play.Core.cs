using UnityEngine;

public static partial class InterludeSfx
{
    public const float WorldHearMinMeters = AudioWorldConstants.HearMinMeters;
    public const float WorldHearMaxMeters = AudioWorldConstants.HearMaxMeters;

    public static bool IsCastPlaying =>
        InterludeSfxBridge.IsCastPlaying != null && InterludeSfxBridge.IsCastPlaying();

    public static float CastRemainingSec =>
        InterludeSfxBridge.CastRemainingSec != null ? InterludeSfxBridge.CastRemainingSec() : 0f;

    public static bool Play(string resourcesPath, float volume = 0.85f) =>
        InterludeSfxBridge.PlayResourcesPath != null
        && InterludeSfxBridge.PlayResourcesPath(resourcesPath, volume);

    public static void Play(AudioClip clip, float volume = 0.85f) =>
        InterludeSfxBridge.PlayClip2D?.Invoke(clip, AudioCategory.Sfx, volume);

    public static void PlayAt(AudioClip clip, Vector3 worldPos, float volume, bool listener, float maxDistanceMeters = -1f)
    {
        if (maxDistanceMeters > 0f && InterludeSfxBridge.PlayClipAtRanged != null)
        {
            InterludeSfxBridge.PlayClipAtRanged(clip, worldPos, AudioCategory.Sfx, volume, listener, maxDistanceMeters);
            return;
        }

        InterludeSfxBridge.PlayClipAt?.Invoke(clip, worldPos, AudioCategory.Sfx, volume, listener);
    }
}