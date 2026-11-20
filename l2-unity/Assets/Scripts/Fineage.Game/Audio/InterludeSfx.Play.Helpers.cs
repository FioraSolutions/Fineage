using UnityEngine;

public static partial class InterludeSfx
{
    public static void PlayCast(AudioClip clip, float volume = 0.85f) =>
        InterludeSfxBridge.PlayCastClip?.Invoke(clip, volume);

    public static void StopCast() => InterludeSfxBridge.StopCast?.Invoke();

    public static void PlayVoice(AudioClip clip, float volume = 1f) =>
        InterludeSfxBridge.PlayVoiceClip?.Invoke(clip, volume);

    public static void StopVoice() => InterludeSfxBridge.StopVoice?.Invoke();

    public static void PlaceAtCaster(Vector3 worldPos, bool listener) =>
        InterludeSfxBridge.PlaceCasterChannels?.Invoke(worldPos, listener);

    public static void PlayAfter(float delaySeconds, System.Action action) =>
        InterludeSfxBridge.PlayDelayed?.Invoke(delaySeconds, action);
}