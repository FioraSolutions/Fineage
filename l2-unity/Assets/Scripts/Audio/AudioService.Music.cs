using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public partial class AudioService
{
    public const string LoginIntroFmodPath = AudioMusicIds.LoginIntroFmodPath;
    public const string LoginIntroResourcesPath = AudioMusicIds.LoginIntroResourcesPath;

    int _areaMusicPriority = -1;
    EventReference _areaMusicEvent;
    readonly Dictionary<EventReference, EventInstance> _areaMusicInstances = new Dictionary<EventReference, EventInstance>();

    EventInstance _townInstance;
    string _townEventPath;

    EventInstance _loginFmodInstance;
    bool _loginFmodActive;
    AudioSource _musicClipChannel;
    bool _loginClipLoop;

    public int CurrentMusicPriority => _areaMusicPriority;
    public EventReference CurrentMusicEvent => _areaMusicEvent;

    void EnsureMusicClipChannel()
    {
        if (_musicClipChannel != null)
            return;
        _musicClipChannel = EnsureChildSource("Music2D", true);
        _musicClipChannel.loop = true;
    }

    void TickMusicLayers()
    {
        float vol = CategoryVolume(AudioCategory.Music);
        if (_loginFmodActive && _loginFmodInstance.isValid())
            _loginFmodInstance.setVolume(vol);

        if (_musicClipChannel != null && _musicClipChannel.isPlaying)
            _musicClipChannel.volume = vol;

        if (_townInstance.isValid())
        {
            _townInstance.setVolume(vol);
            _townInstance.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.STOPPED && !string.IsNullOrEmpty(_townEventPath))
                _townInstance.start();
        }

        foreach (KeyValuePair<EventReference, EventInstance> pair in _areaMusicInstances)
        {
            if (pair.Value.isValid())
                pair.Value.setVolume(vol);
        }
    }

    public void PlayAreaMusic(EventReference musicEvent, int priority)
    {
        if (_disabled || musicEvent.IsNull)
        {
            if (musicEvent.IsNull)
                LogAudioError("AudioService: missing FMOD music event (null EventReference).");
            return;
        }

        if (priority < _areaMusicPriority)
            return;

        if (_areaMusicPriority >= 0
            && !_currentMusicAreaEventEquals(musicEvent)
            && _areaMusicInstances.TryGetValue(_areaMusicEvent, out EventInstance prev)
            && prev.isValid())
        {
            prev.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        _areaMusicPriority = priority;
        _areaMusicEvent = musicEvent;

        if (_areaMusicInstances.TryGetValue(musicEvent, out EventInstance existing) && existing.isValid())
        {
            existing.start();
            return;
        }

        EventInstance created = CreateMusicInstance(musicEvent);
        if (!created.isValid())
            return;

        _areaMusicInstances[musicEvent] = created;
        created.start();
    }

    bool _currentMusicAreaEventEquals(EventReference other) =>
        _areaMusicEvent.Guid.Equals(other.Guid);

    public void StopAreaMusic(EventReference musicEvent)
    {
        if (musicEvent.IsNull)
            return;

        if (_areaMusicInstances.TryGetValue(musicEvent, out EventInstance inst) && inst.isValid())
            inst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ResetMusicPriority() => _areaMusicPriority = -1;

    public void ClearAllMusic()
    {
        ResetMusicPriority();
        StopAreaMusic(_areaMusicEvent);

        foreach (KeyValuePair<EventReference, EventInstance> pair in _areaMusicInstances)
        {
            if (pair.Value.isValid())
            {
                pair.Value.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                pair.Value.release();
            }
        }

        _areaMusicInstances.Clear();
        StopTownTheme();
        StopLoginIntro();
    }

    EventInstance CreateMusicInstance(EventReference musicEvent)
    {
        EventInstance inst = RuntimeManager.CreateInstance(musicEvent);
        if (!inst.isValid())
        {
            LogAudioError("AudioService: missing FMOD music event id=\"" + musicEvent + "\"");
            return default;
        }

        inst.setVolume(CategoryVolume(AudioCategory.Music));
        return inst;
    }

    public void PlayTownTheme(string fmodEventPath)
    {
        if (_disabled || string.IsNullOrEmpty(fmodEventPath))
            return;

        if (_townEventPath == fmodEventPath && _townInstance.isValid())
            return;

        StopTownTheme();

        EventReference er = RuntimeManager.PathToEventReference(fmodEventPath);
        if (er.IsNull)
        {
            LogAudioError("AudioService: missing FMOD town music event id=\"" + fmodEventPath + "\"");
            return;
        }

        _townInstance = RuntimeManager.CreateInstance(er);
        if (!_townInstance.isValid())
        {
            LogAudioError("AudioService: failed to create town music instance id=\"" + fmodEventPath + "\"");
            return;
        }

        _townInstance.setVolume(CategoryVolume(AudioCategory.Music));
        _townInstance.start();
        _townEventPath = fmodEventPath;
    }

    public void StopTownTheme()
    {
        _townEventPath = null;
        if (!_townInstance.isValid())
            return;

        _townInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _townInstance.release();
        _townInstance.clearHandle();
    }

    public void PlayLoginIntro(bool loop)
    {
        if (_disabled)
            return;

        StopLoginIntro();

        if (loop)
        {
            if (TryStartLoginFmod())
                return;
        }

        if (PlayLoginResourcesClip(loop))
            return;

        if (!loop && TryStartLoginFmod())
            return;

        LogAudioError(
            "AudioService: missing login intro (FMOD id=\"" + LoginIntroFmodPath
            + "\" and Resources=\"" + LoginIntroResourcesPath + "\")");
    }

    bool TryStartLoginFmod()
    {
        EventReference er = RuntimeManager.PathToEventReference(LoginIntroFmodPath);
        if (er.IsNull)
        {
            LogAudioError("AudioService: missing FMOD music event id=\"" + LoginIntroFmodPath + "\"");
            return false;
        }

        _loginFmodInstance = RuntimeManager.CreateInstance(er);
        if (!_loginFmodInstance.isValid())
        {
            LogAudioError("AudioService: failed to create FMOD login intro id=\"" + LoginIntroFmodPath + "\"");
            return false;
        }

        _loginFmodInstance.setVolume(CategoryVolume(AudioCategory.Music));
        _loginFmodInstance.start();
        _loginFmodActive = true;
        return true;
    }

    bool PlayLoginResourcesClip(bool loop)
    {
        if (!TryLoadClip(LoginIntroResourcesPath, out AudioClip clip, out string error))
        {
            if (!string.IsNullOrEmpty(error))
                LogAudioError(error);
            return false;
        }

        EnsureMusicClipChannel();
        _loginClipLoop = loop;
        _musicClipChannel.Stop();
        _musicClipChannel.clip = clip;
        _musicClipChannel.loop = loop;
        _musicClipChannel.volume = CategoryVolume(AudioCategory.Music);
        _musicClipChannel.Play();
        return true;
    }

    public void StopLoginIntro()
    {
        if (_loginFmodActive)
        {
            if (_loginFmodInstance.isValid())
            {
                _loginFmodInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _loginFmodInstance.release();
                _loginFmodInstance.clearHandle();
            }

            _loginFmodActive = false;
        }

        if (_musicClipChannel != null && _musicClipChannel.isPlaying)
            _musicClipChannel.Stop();
    }
}