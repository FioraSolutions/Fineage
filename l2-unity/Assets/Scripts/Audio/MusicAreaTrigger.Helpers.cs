using FMODUnity;
using System.Collections;
using UnityEngine;
using System;
using FMOD.Studio;

public partial class MusicAreaTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other) {
        // Debug.Log("Exit area:" + transform.name);
        MusicManager music = RequiredAudioServices.RequireMusic(nameof(MusicAreaTrigger));
        if (music == null)
            return;
        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
            _loopCoroutine = null;
        }

        // Retail: fade out on area leave (FMOD ALLOWFADEOUT), then drop trigger priority.
        if (MusicAreaExitFade.UseAllowFadeoutOnAreaExit)
            music.StopMusic(_enterEvent);
        if (MusicAreaExitFade.ShouldResetPriorityOnExit(MusicAreaExitFade.UseAllowFadeoutOnAreaExit))
            music.ResetPriority();
    }

    private void PlayMusic() {
        OfficialMusicVolumeTable.Load();
        if (OfficialMusicVolumeTable.HasVolumes)
            return;

        MusicManager music = RequiredAudioServices.RequireMusic(nameof(MusicAreaTrigger));
        if (music == null)
            return;
        music.PlayMusic(_enterEvent, _areaPrority);
        if (_loopCoroutine != null) {
            StopCoroutine(_loopCoroutine);
        }
        _loopCoroutine = StartCoroutine(MusicLoop());
    }

    IEnumerator MusicLoop() {
        yield return new WaitForSeconds(_musicLengthSeconds);
        yield return new WaitForSeconds(_loopDelaySeconds);

        PlayMusic();
    }

    private void OnTriggerStay(Collider other) {
        OfficialMusicVolumeTable.Load();
        if (OfficialMusicVolumeTable.HasVolumes)
            return;

        MusicManager music = RequiredAudioServices.RequireMusic(nameof(MusicAreaTrigger));
        if (music == null)
            return;
        if(music.CurrentEventPriority > _areaPrority) {
            music.StopMusic(_enterEvent);
        } else if(music.CurrentMusicEvent.Guid != _enterEvent.Guid) {
            music.PlayMusic(_enterEvent, _areaPrority);
        }
    }
}
