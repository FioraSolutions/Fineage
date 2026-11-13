using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Dark Elf village theme. Retail plays <c>ct_darkelf</c> for that race;
/// the 20_18 map has no music volume of its own, so the theme runs while
/// the camera is still under the cave mouth and stops on the surface.
/// </summary>
public sealed class L2DarkElfVillageMusic : MonoBehaviour
{
    public const string EventPath = "event:/Music/l2_lobby/ct_darkelf";
    const float CaveLipUnityY = 45f;

    EventInstance _inst;
    bool _playing;

    void OnDisable() => Stop();

    void LateUpdate()
    {
        bool inVillage = InVillage();
        if (inVillage && !_playing)
            Play();
        else if (!inVillage && _playing)
            Stop();
        else if (_playing)
            KeepAlive();
    }

    static bool InVillage()
    {
        Scene tile = SceneManager.GetSceneByName("20_18");
        if (!tile.IsValid() || !tile.isLoaded)
            return false;

        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null)
            return player.transform.position.y < CaveLipUnityY;

        Camera cam = Camera.main;
        return cam != null && cam.transform.position.y < CaveLipUnityY;
    }

    void Play()
    {
        try
        {
            EventReference er = RuntimeManager.PathToEventReference(EventPath);
            if (er.IsNull)
                return;

            _inst = RuntimeManager.CreateInstance(er);
            _inst.setVolume(GameSettings.Music);
            _inst.start();
            _playing = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[DarkElfVillage] music failed: " + ex.Message);
        }
    }

    void KeepAlive()
    {
        if (!_inst.isValid())
        {
            _playing = false;
            return;
        }

        _inst.setVolume(GameSettings.Music);
        _inst.getPlaybackState(out PLAYBACK_STATE state);
        if (state == PLAYBACK_STATE.STOPPED)
            _inst.start();
    }

    void Stop()
    {
        if (!_playing)
            return;

        _playing = false;
        if (!_inst.isValid())
            return;

        _inst.stop(global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _inst.release();
        _inst.clearHandle();
    }
}
