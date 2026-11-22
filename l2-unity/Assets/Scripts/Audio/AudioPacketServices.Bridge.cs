using System;
using UnityEngine;

/// <summary>Wires <see cref="InterludeSfxBridge"/> to <see cref="AudioService"/>.</summary>
public static partial class AudioPacketServicesBridge
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void WireInterludeBridge()
    {
        InterludeSfxBridge.LoadClip = resourcesPath =>
        {
            AudioService svc = AudioPacketServices.Service;
            if (svc == null)
                return null;
            return svc.TryLoadClip(resourcesPath, out AudioClip clip, out _) ? clip : null;
        };

        InterludeSfxBridge.PlayResourcesPath = (path, vol) =>
        {
            AudioService svc = AudioPacketServices.Service;
            return svc != null && svc.PlayResourcesPath(path, vol);
        };

        InterludeSfxBridge.PlayClip2D = (clip, category, vol) =>
        {
            AudioService svc = AudioPacketServices.Service;
            if (svc == null || clip == null)
                return;
            svc.PlayLoadedClip2D(clip, category, vol, clip.name);
        };

        InterludeSfxBridge.PlayClipAt = (clip, pos, category, vol, listener) =>
        {
            AudioService svc = AudioPacketServices.Service;
            if (svc == null || clip == null)
                return;
            svc.PlayClipAt(clip, pos, category, vol, listener, clip.name);
        };

        InterludeSfxBridge.PlayClipAtRanged = (clip, pos, category, vol, listener, innerRadiusUu) =>
        {
            AudioService svc = AudioPacketServices.Service;
            if (svc == null || clip == null)
                return;
            svc.PlayClipAt(clip, pos, category, vol, listener, clip.name, innerRadiusUu);
        };

        InterludeSfxBridge.PlayCastClip = (clip, vol) =>
        {
            AudioService svc = AudioPacketServices.Service;
            if (svc == null || clip == null)
                return;
            svc.PlayCastClipByClip(clip, vol);
        };

        InterludeSfxBridge.StopCast = () => AudioPacketServices.Service?.StopCast();

        InterludeSfxBridge.PlayVoiceClip = (clip, vol) =>
        {
            AudioService svc = AudioPacketServices.Service;
            if (svc == null || clip == null)
                return;
            svc.PlayVoiceClipInternal(clip, vol, clip.name);
        };

        InterludeSfxBridge.StopVoice = () => AudioPacketServices.Service?.StopVoice();

        InterludeSfxBridge.PlaceCasterChannels = (pos, listener) =>
            AudioPacketServices.Service?.PlaceCasterChannels(pos, listener);

        InterludeSfxBridge.PlayDelayed = (delay, action) =>
            AudioPacketServices.Service?.PlayDelayed(delay, action);

        InterludeSfxBridge.IsCastPlaying = () =>
            AudioPacketServices.Service != null && AudioPacketServices.Service.IsCastPlaying;

        InterludeSfxBridge.CastRemainingSec = () =>
            AudioPacketServices.Service != null ? AudioPacketServices.Service.CastRemainingSec : 0f;

        InterludeSfxBridge.PlayHitImpact = critical =>
        {
            AudioService svc = AudioPacketServices.Service;
            return svc != null && svc.PlayHitImpact(critical, Vector3.zero);
        };

        LoginSplashMusicBridge.PlaySplashOnce = () => AudioPacketServices.Service?.PlayLoginIntro(loop: false);
        LoginSplashMusicBridge.Stop = () => AudioPacketServices.Service?.StopLoginIntro();
    }
}