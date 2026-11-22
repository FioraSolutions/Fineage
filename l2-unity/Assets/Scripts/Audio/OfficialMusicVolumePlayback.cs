using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Plays official MusicVolume tracks (musicinfo → Music.bank) when the listener
/// is inside the volume's official brush. Replaces L2TownMusic hardcoded rings.
/// </summary>
public sealed class OfficialMusicVolumePlayback : MonoBehaviour
{
    const float TickSeconds = 0.25f;

    string _playingPath;
    string _playingVolumeId;
    readonly HashSet<string> _loggedMissing = new HashSet<string>();
    float _next;

    void OnEnable()
    {
        OfficialMusicVolumeTable.Load();
    }

    void OnDisable()
    {
        StopTheme();
    }

    void LateUpdate()
    {
        if (Time.unscaledTime < _next)
            return;
        _next = Time.unscaledTime + TickSeconds;

        if (!OfficialMusicVolumeTable.HasVolumes)
            return;
        if (!AudioWorldGate.TryGetListenerPosition(out Vector3 unity))
        {
            StopTheme();
            return;
        }

        Vector3 ue = VectorUtils.ConvertPosUnityToL2j(unity);
        VectorUtils.GetSwMapTile(ue, out int tileX, out int tileY);
        Scene scene = SceneManager.GetActiveScene();
        OfficialMusicVolumeTable.Volume volume = OfficialMusicVolumeTable.FindContainingNearby(
            ue, tileX, tileY, scene.IsValid() ? scene.name : null);

        if (volume == null)
        {
            StopTheme();
            return;
        }

        int musicId = volume.musicId;
        WorldClock clock = WorldClockPacketServices.Clock;
        if (volume.hasNightMusic && clock != null && clock.IsNightTime() && volume.nightMusicId != 0)
            musicId = volume.nightMusicId;

        if (!TryResolveEvent(volume, musicId, out string eventPath))
        {
            StopTheme();
            return;
        }

        if (_playingPath == eventPath && _playingVolumeId == volume.Id)
            return;

        AudioService svc = AudioPacketServices.Service;
        if (svc == null)
            return;
        svc.PlayTownTheme(eventPath);
        _playingPath = eventPath;
        _playingVolumeId = volume.Id;
    }

    bool TryResolveEvent(OfficialMusicVolumeTable.Volume volume, int musicId, out string eventPath)
    {
        eventPath = null;
        IReadOnlyList<string> tracks = OfficialMusicInfo.TracksFor(musicId);
        if (tracks.Count == 0)
        {
            LogOnce("id:" + musicId, "OfficialMusicVolumePlayback: musicinfo has no tracks for nMusicID="
                + musicId + " map=\"" + volume.map + "\" name=\"" + volume.name + "\"");
            return false;
        }

        for (int i = 0; i < tracks.Count; i++)
        {
            if (OfficialMusicBankEvents.TryResolve(tracks[i], out eventPath, out _))
                return true;
        }

        LogOnce("tracks:" + musicId, "OfficialMusicVolumePlayback: no Music.bank event for nMusicID="
            + musicId + " map=\"" + volume.map + "\" name=\"" + volume.name
            + "\" tracks=" + string.Join(",", tracks));
        return false;
    }

    void StopTheme()
    {
        if (_playingPath == null)
            return;
        _playingPath = null;
        _playingVolumeId = null;
        AudioPacketServices.Service?.StopTownTheme();
    }

    void LogOnce(string key, string message)
    {
        if (!_loggedMissing.Add(key))
            return;
        Debug.LogError(message);
    }
}
