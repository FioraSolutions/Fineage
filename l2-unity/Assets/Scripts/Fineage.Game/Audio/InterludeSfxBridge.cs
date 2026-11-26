using System;
using UnityEngine;

/// <summary>Indirection so Fineage.Game avoids a cyclic asm reference to AudioService.</summary>
public static class InterludeSfxBridge
{
    public static Func<string, AudioClip> LoadClip;
    public static Func<string, float, bool> PlayResourcesPath;
    public static Action<AudioClip, AudioCategory, float> PlayClip2D;
    public static Action<AudioClip, Vector3, AudioCategory, float, bool> PlayClipAt;
    public static Action<AudioClip, Vector3, AudioCategory, float, bool, float> PlayClipAtRanged;
    public static Action<AudioClip, float> PlayCastClip;
    public static Action StopCast;
    public static Action<AudioClip, float> PlayVoiceClip;
    public static Action StopVoice;
    public static Action<Vector3, bool> PlaceCasterChannels;
    public static Action<float, Action> PlayDelayed;
    public static Func<bool> IsCastPlaying;
    public static Func<float> CastRemainingSec;
    public static Func<bool, bool> PlayHitImpact;
}